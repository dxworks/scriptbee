import * as vscode from 'vscode';

class Logger {
  private static instance: Logger;
  private outputChannel: vscode.OutputChannel | undefined;

  private constructor() {}

  public static getInstance(): Logger {
    if (!Logger.instance) {
      Logger.instance = new Logger();
    }
    return Logger.instance;
  }

  public init() {
    this.outputChannel = vscode.window.createOutputChannel('ScriptBee');
  }

  public log(message: string) {
    if (this.outputChannel) {
      const timestamp = new Date().toISOString();
      this.outputChannel.appendLine(`[${timestamp}] [INFO] ${message}`);
    }
  }

  public warn(message: string, error?: unknown) {
    if (this.outputChannel) {
      const timestamp = new Date().toISOString();
      this.outputChannel.appendLine(`[${timestamp}] [WARN] ${message}`);
      if (error) {
        this.printError(this.outputChannel, timestamp, error);
      }
    }
  }

  public error(message: string, error?: unknown) {
    if (this.outputChannel) {
      const timestamp = new Date().toISOString();
      this.outputChannel.appendLine(`[${timestamp}] [ERROR] ${message}`);
      if (error) {
        this.printError(this.outputChannel, timestamp, error);
      }
    }
  }

  private printError(outputChannel: vscode.OutputChannel, timestamp: string, error: any) {
    if (error instanceof Error) {
      outputChannel.appendLine(`[${timestamp}] [ERROR] Details: ${error.stack || error.message}`);
    } else {
      outputChannel.appendLine(`[${timestamp}] [ERROR] Details: ${String(error)}`);
    }
  }

  public show() {
    if (this.outputChannel) {
      this.outputChannel.show();
    }
  }
}

export const logger = Logger.getInstance();
