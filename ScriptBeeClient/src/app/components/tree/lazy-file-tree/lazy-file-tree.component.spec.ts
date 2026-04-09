import { beforeEach, describe, expect, it, vi } from 'vitest';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FileTreeNode, LazyFileTreeComponent } from './lazy-file-tree.component';
import { MatTreeModule } from '@angular/material/tree';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { TreeActionsMenuComponent } from '../selectable-tree/tree-actions-menu/tree-actions-menu.component';
import { By } from '@angular/platform-browser';
import { of } from 'rxjs';
import { TreeNode } from '../../../types/tree-node';

interface TestData {
  id: string;
  hasChildren: boolean;
  tags: string[];
}

describe('LazyFileTreeComponent', () => {
  let component: LazyFileTreeComponent<TestData>;
  let fixture: ComponentFixture<LazyFileTreeComponent<TestData>>;

  const mockData: FileTreeNode<TestData>[] = [
    {
      name: 'Folder 1',
      data: { id: '1', hasChildren: true, tags: ['important'] },
    },
    {
      name: 'File 2',
      data: { id: '2', hasChildren: false, tags: ['readonly'] },
    },
  ];

  const childrenAccessor = (node: FileTreeNode<TestData>) => {
    if (!('isVirtual' in node) && node.data?.id === '1') {
      return of([
        {
          name: 'File 1.1',
          data: { id: '1.1', hasChildren: false, tags: ['child'] },
        },
      ] as FileTreeNode<TestData>[]);
    }
    return of([]);
  };

  const hasChildAccessor = (node: TreeNode<TestData>) => node.data.hasChildren;
  const idAccessor = (node: TreeNode<TestData>) => node.data.id;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LazyFileTreeComponent, MatTreeModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule, TreeActionsMenuComponent],
    }).compileComponents();

    fixture = TestBed.createComponent<LazyFileTreeComponent<TestData>>(LazyFileTreeComponent);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('data', mockData);
    fixture.componentRef.setInput('childrenAccessor', childrenAccessor);
    fixture.componentRef.setInput('hasChildAccessor', hasChildAccessor);
    fixture.componentRef.setInput('idAccessor', idAccessor);
    fixture.detectChanges();
  });

  it('should render root nodes', () => {
    const treeText = fixture.nativeElement.textContent;
    expect(treeText).toContain('Folder 1');
    expect(treeText).toContain('File 2');
  });

  it('should emit clickChange with node data when a file is clicked', () => {
    const clickSpy = vi.spyOn(component.clickChange, 'emit');

    const leafNodes = fixture.debugElement.queryAll(By.css('.leaf-node'));
    const file2Node = leafNodes.find((n) => n.nativeElement.textContent.includes('File 2'));

    expect(file2Node).toBeTruthy();
    file2Node!.nativeElement.click();

    expect(clickSpy).toHaveBeenCalledWith(mockData[1]);
    expect(clickSpy.mock.calls[0][0].data?.tags).toContain('readonly');
  });

  it('should emit expand with node data when a folder is toggled', () => {
    const expandSpy = vi.spyOn(component.expand, 'emit');

    const toggleButton = fixture.debugElement.query(By.css('button[matTreeNodeToggle]'));
    toggleButton.nativeElement.click();
    fixture.detectChanges();

    expect(expandSpy).toHaveBeenCalledWith(mockData[0]);
    expect(expandSpy.mock.calls[0][0].data?.tags).toContain('important');
  });

  it('should render loading spinner when virtual node is present', () => {
    fixture.componentRef.setInput('data', [...mockData, { isVirtual: true, parentId: '1', state: 'loading' }]);
    fixture.detectChanges();

    const spinner = fixture.debugElement.query(By.css('mat-spinner'));
    expect(spinner).toBeTruthy();
  });
});
