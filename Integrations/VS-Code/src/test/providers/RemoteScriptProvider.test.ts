import * as assert from 'assert';
import * as vscode from 'vscode';
import * as sinon from 'sinon';
import { RemoteScriptProvider } from '../../providers/RemoteScriptProvider';
import * as projectFiles from '../../api/projectFiles';

suite('RemoteScriptProvider Tests', () => {
  let provider: RemoteScriptProvider;
  let getScriptContentStub: sinon.SinonStub;

  setup(() => {
    provider = new RemoteScriptProvider();
    getScriptContentStub = sinon.stub(projectFiles, 'getScriptContent');
  });

  teardown(() => {
    sinon.restore();
  });

  test('provideTextDocumentContent parses URI and fetches content', async () => {
    const projectId = 'proj-123';
    const fileId = 'file-456';
    const baseUrl = 'http://localhost:5000';
    const expectedContent = 'test script content';

    const uri = vscode.Uri.parse(`${RemoteScriptProvider.scheme}://remote/${projectId}/${fileId}?baseUrl=${encodeURIComponent(baseUrl)}`);

    getScriptContentStub.resolves(expectedContent);

    const content = await provider.provideTextDocumentContent(uri);

    assert.strictEqual(content, expectedContent);
    assert.ok(getScriptContentStub.calledWith(baseUrl, projectId, fileId));
  });

  test('provideTextDocumentContent returns error message on API failure', async () => {
    const uri = vscode.Uri.parse(`${RemoteScriptProvider.scheme}://remote/p1/f1?baseUrl=http://api`);

    getScriptContentStub.rejects(new Error('API Error'));

    const content = await provider.provideTextDocumentContent(uri);

    assert.ok(content.includes('Failed to load remote script'));
    assert.ok(content.includes('API Error'));
  });

  test('provideTextDocumentContent handles invalid URIs', async () => {
    const uri = vscode.Uri.parse(`${RemoteScriptProvider.scheme}://remote/missing-segments`);

    const content = await provider.provideTextDocumentContent(uri);

    assert.ok(content.includes('Invalid remote script URI'));
  });
});
