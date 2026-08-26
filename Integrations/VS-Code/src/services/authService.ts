import * as vscode from 'vscode';
import * as http from 'node:http';
import * as net from 'node:net';
import * as crypto from 'node:crypto';
import axios from 'axios';
import { Connection, storage } from '../utils/storage';
import { logger } from '../utils/logger';

export interface WebAuthConfig {
  authMode?: string;
  authority?: string;
  authWellknownEndpointUrl?: string;
  clientId?: string;
  scope?: string;
}

export interface OpenIdConfiguration {
  authorization_endpoint: string;
  token_endpoint: string;
  end_session_endpoint?: string;
}

export interface StoredTokenData {
  accessToken: string;
  refreshToken?: string;
  expiresAt?: number;
  tokenType?: string;
}

interface TokenEndpointResponse {
  access_token: string;
  refresh_token?: string;
  expires_in?: number;
  token_type?: string;
}

export class AuthService {
  private static readonly tokenStoragePrefix = 'scriptbee.auth.token.';
  private static readonly configCache: Map<string, WebAuthConfig> = new Map();

  public generateCodeVerifier(): string {
    return crypto.randomBytes(32).toString('base64url');
  }

  public generateCodeChallenge(verifier: string): string {
    return crypto.createHash('sha256').update(verifier).digest('base64url');
  }

  public generateRandomState(): string {
    return crypto.randomBytes(16).toString('base64url');
  }

  public async getAuthConfig(serverUrl: string): Promise<WebAuthConfig | null> {
    try {
      const normalizedUrl = serverUrl.replace(/\/+$/, '');
      const configUrl = `${normalizedUrl}/api/config/auth`;
      const response = await axios.get<WebAuthConfig>(configUrl);
      if (response.data) {
        AuthService.configCache.set(normalizedUrl, response.data);
        return response.data;
      }
      return null;
    } catch (error) {
      logger.error('Failed to fetch auth configuration from server', error);
      return null;
    }
  }

  public async getOpenIdConfiguration(authConfig: WebAuthConfig): Promise<OpenIdConfiguration | null> {
    const discoveryUrl =
      authConfig.authWellknownEndpointUrl || (authConfig.authority ? `${authConfig.authority.replace(/\/+$/, '')}/.well-known/openid-configuration` : null);

    if (!discoveryUrl) {
      return null;
    }

    try {
      const response = await axios.get<OpenIdConfiguration>(discoveryUrl);
      return response.data;
    } catch (error) {
      logger.error('Failed to fetch OpenID configuration from discovery endpoint', error);
      if (authConfig.authority) {
        const authorityUrl = authConfig.authority.replace(/\/+$/, '');
        return {
          authorization_endpoint: `${authorityUrl}/protocol/openid-connect/auth`,
          token_endpoint: `${authorityUrl}/protocol/openid-connect/token`,
        };
      }
      return null;
    }
  }

  public async login(connection: Connection): Promise<boolean> {
    const authConfig = await this.getAuthConfig(connection.url);
    if (!authConfig) {
      throw new Error('Unable to retrieve authentication configuration from the ScriptBee server.');
    }

    if (authConfig.authMode?.toLowerCase() === 'development') {
      return true;
    }

    const openIdConfig = await this.getOpenIdConfiguration(authConfig);
    if (!openIdConfig || !openIdConfig.authorization_endpoint || !openIdConfig.token_endpoint) {
      throw new Error('Unable to resolve OpenID authorization and token endpoints.');
    }

    const clientId = authConfig.clientId;
    if (!clientId) {
      throw new Error('Client ID is missing in authentication configuration.');
    }

    const codeVerifier = this.generateCodeVerifier();
    const codeChallenge = this.generateCodeChallenge(codeVerifier);
    const state = this.generateRandomState();

    const { server, port } = await this.startLoopbackServer();
    const redirectUri = `http://127.0.0.1:${port}/callback`;

    try {
      const codePromise = this.waitForAuthCallback(server, state);

      const authUrl = new URL(openIdConfig.authorization_endpoint);
      authUrl.searchParams.set('response_type', 'code');
      authUrl.searchParams.set('client_id', clientId);
      authUrl.searchParams.set('redirect_uri', redirectUri);
      authUrl.searchParams.set('scope', authConfig.scope || 'openid profile email');
      authUrl.searchParams.set('state', state);
      authUrl.searchParams.set('code_challenge', codeChallenge);
      authUrl.searchParams.set('code_challenge_method', 'S256');

      await vscode.env.openExternal(vscode.Uri.parse(authUrl.toString()));

      const authorizationCode = await codePromise;
      const tokens = await this.exchangeCodeForTokens(openIdConfig.token_endpoint, clientId, authorizationCode, redirectUri, codeVerifier);

      await this.saveTokens(connection.id, tokens);
      return true;
    } finally {
      server.close();
    }
  }

  public async getAccessToken(connection: Connection): Promise<string | undefined> {
    const tokenData = await this.getStoredTokenData(connection.id);
    if (!tokenData) {
      return undefined;
    }

    const now = Date.now();
    const isExpired = tokenData.expiresAt ? now >= tokenData.expiresAt - 60000 : false;

    if (!isExpired) {
      return tokenData.accessToken;
    }

    if (tokenData.refreshToken) {
      try {
        const refreshedTokenData = await this.refreshToken(connection, tokenData.refreshToken);
        if (refreshedTokenData) {
          return refreshedTokenData.accessToken;
        }
      } catch (error) {
        logger.error('Failed to refresh authentication token', error);
      }
    }

    return tokenData.accessToken;
  }

  public async setAccessToken(connectionId: string, token: string): Promise<void> {
    const tokenData: StoredTokenData = {
      accessToken: token,
    };
    await this.saveStoredTokenData(connectionId, tokenData);
  }

  public async logout(connectionId: string): Promise<void> {
    await storage.deleteSecret(`${AuthService.tokenStoragePrefix}${connectionId}`);
  }

  public async isAuthenticated(connectionId: string): Promise<boolean> {
    const tokenData = await this.getStoredTokenData(connectionId);
    return Boolean(tokenData?.accessToken);
  }

  public async getStoredTokenData(connectionId: string): Promise<StoredTokenData | undefined> {
    const secret = await storage.getSecret(`${AuthService.tokenStoragePrefix}${connectionId}`);
    if (!secret) {
      return undefined;
    }

    try {
      return JSON.parse(secret) as StoredTokenData;
    } catch {
      return { accessToken: secret };
    }
  }

  public async saveStoredTokenData(connectionId: string, tokenData: StoredTokenData): Promise<void> {
    await storage.storeSecret(`${AuthService.tokenStoragePrefix}${connectionId}`, JSON.stringify(tokenData));
  }

  private async startLoopbackServer(): Promise<{ server: http.Server; port: number }> {
    const server = http.createServer();
    await new Promise<void>((resolve, reject) => {
      server.listen(0, '127.0.0.1', () => resolve());
      server.on('error', reject);
    });

    const address = server.address() as net.AddressInfo;
    return { server, port: address.port };
  }

  private waitForAuthCallback(server: http.Server, expectedState: string): Promise<string> {
    return new Promise<string>((resolve, reject) => {
      const timeoutId = setTimeout(() => {
        reject(new Error('Authentication timed out after 2 minutes.'));
      }, 120000);

      server.on('request', (req: http.IncomingMessage, res: http.ServerResponse) => {
        const reqUrl = new URL(req.url || '', 'http://127.0.0.1');
        if (reqUrl.pathname !== '/callback') {
          res.writeHead(404, { 'Content-Type': 'text/plain' });
          res.end('Not Found');
          return;
        }

        const receivedState = reqUrl.searchParams.get('state');
        const code = reqUrl.searchParams.get('code');
        const error = reqUrl.searchParams.get('error');
        const errorDescription = reqUrl.searchParams.get('error_description');

        if (error) {
          clearTimeout(timeoutId);
          res.writeHead(400, { 'Content-Type': 'text/html; charset=utf-8' });
          res.end(
            `<html><body style="font-family: sans-serif; text-align: center; padding: 40px;"><h2>Authentication Failed</h2><p>${error}: ${errorDescription || ''}</p></body></html>`
          );
          reject(new Error(`Authentication failed: ${error} - ${errorDescription || ''}`));
          return;
        }

        if (receivedState !== expectedState || !code) {
          clearTimeout(timeoutId);
          res.writeHead(400, { 'Content-Type': 'text/html; charset=utf-8' });
          res.end(
            '<html><body style="font-family: sans-serif; text-align: center; padding: 40px;"><h2>Authentication Failed</h2><p>Invalid authorization state or missing code.</p></body></html>'
          );
          reject(new Error('Invalid authorization state or missing code.'));
          return;
        }

        clearTimeout(timeoutId);
        res.writeHead(200, { 'Content-Type': 'text/html; charset=utf-8' });
        res.end(
          '<html><body style="font-family: sans-serif; text-align: center; padding: 40px;"><h2>Authentication Successful</h2><p>You can close this tab and return to Visual Studio Code.</p></body></html>'
        );
        resolve(code);
      });
    });
  }

  private async exchangeCodeForTokens(
    tokenEndpoint: string,
    clientId: string,
    code: string,
    redirectUri: string,
    codeVerifier: string
  ): Promise<StoredTokenData> {
    const params = new URLSearchParams();
    params.set('grant_type', 'authorization_code');
    params.set('client_id', clientId);
    params.set('code', code);
    params.set('redirect_uri', redirectUri);
    params.set('code_verifier', codeVerifier);

    const response = await axios.post<TokenEndpointResponse>(tokenEndpoint, params.toString(), {
      headers: {
        'Content-Type': 'application/x-www-form-urlencoded',
      },
    });

    const data = response.data;
    const expiresAt = data.expires_in ? Date.now() + data.expires_in * 1000 : undefined;

    return {
      accessToken: data.access_token,
      refreshToken: data.refresh_token,
      expiresAt,
      tokenType: data.token_type || 'Bearer',
    };
  }

  private async refreshToken(connection: Connection, refreshTokenValue: string): Promise<StoredTokenData | null> {
    const authConfig = (await this.getAuthConfig(connection.url)) || AuthService.configCache.get(connection.url.replace(/\/+$/, ''));
    if (!authConfig || !authConfig.clientId) {
      return null;
    }

    const openIdConfig = await this.getOpenIdConfiguration(authConfig);
    if (!openIdConfig || !openIdConfig.token_endpoint) {
      return null;
    }

    const params = new URLSearchParams();
    params.set('grant_type', 'refresh_token');
    params.set('client_id', authConfig.clientId);
    params.set('refresh_token', refreshTokenValue);

    const response = await axios.post<TokenEndpointResponse>(openIdConfig.token_endpoint, params.toString(), {
      headers: {
        'Content-Type': 'application/x-www-form-urlencoded',
      },
    });

    const data = response.data;
    const expiresAt = data.expires_in ? Date.now() + data.expires_in * 1000 : undefined;

    const refreshedData: StoredTokenData = {
      accessToken: data.access_token,
      refreshToken: data.refresh_token || refreshTokenValue,
      expiresAt,
      tokenType: data.token_type || 'Bearer',
    };

    await this.saveTokens(connection.id, refreshedData);
    return refreshedData;
  }

  private async saveTokens(connectionId: string, tokens: StoredTokenData): Promise<void> {
    await this.saveStoredTokenData(connectionId, tokens);
  }
}

export const authService = new AuthService();
