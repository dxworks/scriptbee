import * as assert from 'assert';
import * as vscode from 'vscode';
import * as sinon from 'sinon';
import { hideMetaFiles } from '../../utils/workspaceUtils';

suite('Workspace Utils Tests', () => {
  let getConfigurationStub: sinon.SinonStub;
  let updateStub: sinon.SinonStub;

  setup(() => {
    updateStub = sinon.stub();
    getConfigurationStub = sinon.stub(vscode.workspace, 'getConfiguration').returns({
      get: (key: string) => {
        if (key === 'exclude') {
          return { node_modules: true };
        }
        return undefined;
      },
      update: updateStub,
    } as any);
  });

  teardown(() => {
    sinon.restore();
  });

  test('hideMetaFiles should update configuration to exclude .sb.meta files', async () => {
    sinon.stub(vscode.workspace, 'workspaceFolders').value(undefined);

    await hideMetaFiles();

    assert.ok(updateStub.calledOnce);

    const [key, value, target] = updateStub.getCall(0).args;
    assert.strictEqual(key, 'exclude');
    assert.ok(value['**/*.sb.meta']);
    assert.strictEqual(target, vscode.ConfigurationTarget.Global);
  });
});
