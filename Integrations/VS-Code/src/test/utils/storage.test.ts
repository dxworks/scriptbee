import * as assert from 'assert';
import * as vscode from 'vscode';
import * as sinon from 'sinon';
import * as fs from 'fs/promises';
import * as os from 'os';
import * as path from 'path';
import { storage } from '../../utils/storage';

suite('Storage Utils Tests', () => {
  let testRoot: string;

  setup(async () => {
    testRoot = path.join(os.tmpdir(), `scriptbee-storage-test-${Date.now()}`);
    await fs.mkdir(testRoot, { recursive: true });
  });

  teardown(async () => {
    sinon.restore();
    await fs.rm(testRoot, { recursive: true, force: true });
  });

  test('saveScriptMeta constructs adjacent .sb.meta file correctly', async () => {
    const fileUri = vscode.Uri.file(path.join(testRoot, 'my_script.js'));
    const meta = { id: 'test-id', type: 'file' as const };

    await storage.saveScriptMeta(fileUri, meta);

    const expectedMetaPath = `${fileUri.fsPath}.sb.meta`;
    const exists = await fs
      .access(expectedMetaPath)
      .then(() => true)
      .catch(() => false);
    assert.ok(exists);

    const writtenContent = await fs.readFile(expectedMetaPath, 'utf8');
    assert.deepStrictEqual(JSON.parse(writtenContent), meta);
  });

  test('getScriptMeta retrieves data from adjacent .sb.meta file', async () => {
    const fileUri = vscode.Uri.file(path.join(testRoot, 'existing.js'));
    const metaPath = `${fileUri.fsPath}.sb.meta`;
    const metaData = { id: 'existing-id', type: 'file' as const };

    await fs.writeFile(metaPath, JSON.stringify(metaData), 'utf8');

    const retrievedMeta = await storage.getScriptMeta(fileUri);
    assert.deepStrictEqual(retrievedMeta, metaData);
  });
});
