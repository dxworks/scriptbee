import * as assert from 'assert';
import * as sinon from 'sinon';
import * as fs from 'fs/promises';
import * as path from 'path';
import * as os from 'os';
import * as vscode from 'vscode';
import axiosInstance from '../../api/axiosInstance';
import MockAdapter from 'axios-mock-adapter';
import { generateClassesService } from '../../services/generateClassesService';
import * as workspaceUtils from '../../utils/workspaceUtils';

suite('GenerateClassesService Tests', function () {
  this.timeout(10000);
  let mock: MockAdapter;
  let testRoot: string;
  let originalWorkspaceRoot: string | undefined;

  setup(async () => {
    mock = new MockAdapter(axiosInstance);

    const randomId = Math.random().toString(36).substring(2, 10);
    testRoot = path.join(os.tmpdir(), `scriptbee-gen-test-${randomId}`);
    await fs.mkdir(testRoot, { recursive: true });

    const config = vscode.workspace.getConfiguration('scriptbee');
    originalWorkspaceRoot = config.get('workspaceRoot');
    await config.update('workspaceRoot', testRoot, vscode.ConfigurationTarget.Global);

    sinon.stub(workspaceUtils, 'getWorkspaceRoot').returns(testRoot);
  });

  teardown(async () => {
    mock.restore();
    sinon.restore();

    const config = vscode.workspace.getConfiguration('scriptbee');
    await config.update('workspaceRoot', originalWorkspaceRoot, vscode.ConfigurationTarget.Global);

    try {
      if (testRoot) {
        await fs.rm(testRoot, { recursive: true, force: true });
      }
    } catch (e) {}
  });

  function createBinaryFile(filePath: string, content: string): Buffer {
    const pathBytes = Buffer.from(filePath, 'utf8');
    const contentBytes = Buffer.from(content, 'utf8');

    const buffer = Buffer.alloc(4 + pathBytes.length + 8 + contentBytes.length);
    let offset = 0;

    buffer.writeInt32BE(pathBytes.length, offset);
    offset += 4;
    pathBytes.copy(buffer, offset);
    offset += pathBytes.length;

    buffer.writeBigInt64BE(BigInt(contentBytes.length), offset);
    offset += 8;
    contentBytes.copy(buffer, offset);

    return buffer;
  }

  function createEndMarker(): Buffer {
    const buffer = Buffer.alloc(4);
    buffer.writeInt32BE(0, 0);
    return buffer;
  }

  test('generate should parse binary stream and write files', async () => {
    const baseUrl = 'http://localhost:5000';
    const projectId = 'test-proj';
    const instanceId = 'inst-1';

    const file1 = createBinaryFile('models/User.cs', 'public class User {}');
    const file2 = createBinaryFile('README.md', '# Generated Models');
    const end = createEndMarker();

    const combined = Buffer.concat([file1, file2, end]);

    mock.onPost(`${baseUrl}/api/projects/${projectId}/instances/${instanceId}/context/generate-classes`).reply(200, combined);

    await generateClassesService.generate(baseUrl, projectId, instanceId);

    const generatedPath = path.join(testRoot, 'projects', projectId, '.generated');

    const userCsContent = await fs.readFile(path.join(generatedPath, 'models/User.cs'), 'utf8');
    assert.strictEqual(userCsContent, 'public class User {}');

    const readmeContent = await fs.readFile(path.join(generatedPath, 'README.md'), 'utf8');
    assert.strictEqual(readmeContent, '# Generated Models');
  });

  test('generate should clear existing generated files', async () => {
    const baseUrl = 'http://localhost:5000';
    const projectId = 'test-proj-clear';
    const instanceId = 'inst-1';

    const generatedPath = path.join(testRoot, 'projects', projectId, '.generated');
    await fs.mkdir(generatedPath, { recursive: true });
    await fs.writeFile(path.join(generatedPath, 'stale.js'), 'stale content');

    const end = createEndMarker();
    mock.onPost(`${baseUrl}/api/projects/${projectId}/instances/${instanceId}/context/generate-classes`).reply(200, end);

    await generateClassesService.generate(baseUrl, projectId, instanceId);

    const files = await fs.readdir(generatedPath);
    assert.strictEqual(files.length, 0, 'Generated path should be empty (stale file deleted)');
  });
});
