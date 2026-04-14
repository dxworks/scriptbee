import axios from 'axios';
import * as vscode from 'vscode';

export function getErrorMessage(error: unknown): string {
  if (axios.isAxiosError(error)) {
    if (error.response?.data) {
      if (typeof error.response.data.message === 'string') {
        return error.response.data.message;
      }
      if (typeof error.response.data === 'string') {
        return error.response.data;
      }
      return JSON.stringify(error.response.data);
    }
    return error.message;
  }
  if (error instanceof Error) {
    return error.message;
  }
  if (typeof error === 'object' && error !== null) {
    return JSON.stringify(error);
  }
  return String(error) || 'Unknown error occurred';
}

export async function showErrorWithCopy(title: string, error: unknown) {
  const msg = getErrorMessage(error);
  const fullMessage = `${title}: ${msg}`;
  const copyAction = 'Copy Details';

  const result = await vscode.window.showErrorMessage(fullMessage, copyAction);
  if (result === copyAction) {
    await vscode.env.clipboard.writeText(fullMessage);
    vscode.window.setStatusBarMessage('$(clippy) Error details copied to clipboard', 3000);
  }
}
