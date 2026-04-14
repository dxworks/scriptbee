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

  public error(message: string, error?: unknown) {
    if (this.outputChannel) {
      const timestamp = new Date().toISOString();
      this.outputChannel.appendLine(`[${timestamp}] [ERROR] ${message}`);
      if (error) {
        if (error instanceof Error) {
          this.outputChannel.appendLine(`[${timestamp}] [ERROR] Details: ${error.stack || error.message}`);
        } else {
          this.outputChannel.appendLine(`[${timestamp}] [ERROR] Details: ${String(error)}`);
        }
      }
    }
  }

  public show() {
    if (this.outputChannel) {
      this.outputChannel.show();
    }
  }
}

export const logger = Logger.getInstance();
