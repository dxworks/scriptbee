import { beforeEach, describe, expect, it, vi } from 'vitest';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LazyFileTreeComponent } from './lazy-file-tree.component';
import { MatTreeModule } from '@angular/material/tree';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { TreeActionsMenuComponent } from '../selectable-tree/tree-actions-menu/tree-actions-menu.component';
import { By } from '@angular/platform-browser';
import { of, throwError } from 'rxjs';
import { TreeNode } from '../../../types/tree-node';
import { HttpErrorResponse } from '@angular/common/http';

interface TestData {
  id: string;
  name: string;
  hasChildren: boolean;
  tags: string[];
}

describe('LazyFileTreeComponent', () => {
  let component: LazyFileTreeComponent<TestData>;
  let fixture: ComponentFixture<LazyFileTreeComponent<TestData>>;

  const mockData: TreeNode<TestData>[] = [
    {
      data: { id: '1', name: 'Folder 1', hasChildren: true, tags: ['important'] },
    },
    {
      data: { id: '2', name: 'File 2', hasChildren: false, tags: ['readonly'] },
    },
  ];

  let fetchDataMock: ReturnType<typeof vi.fn>;

  const hasChildAccessor = (node: TreeNode<TestData>) => node.data.hasChildren;
  const idAccessor = (node: TreeNode<TestData>) => node.data.id;
  const displayNameAccessor = (node: TreeNode<TestData>) => node.data.name;
  const selectedAccessor = () => false;

  beforeEach(async () => {
    fetchDataMock = vi.fn().mockReturnValue(of({ data: mockData, totalCount: 2 }));

    await TestBed.configureTestingModule({
      imports: [LazyFileTreeComponent, MatTreeModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule, TreeActionsMenuComponent],
    }).compileComponents();

    fixture = TestBed.createComponent<LazyFileTreeComponent<TestData>>(LazyFileTreeComponent);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('fetchData', fetchDataMock);
    fixture.componentRef.setInput('hasChildAccessor', hasChildAccessor);
    fixture.componentRef.setInput('idAccessor', idAccessor);
    fixture.componentRef.setInput('displayNameAccessor', displayNameAccessor);
    fixture.componentRef.setInput('selectedAccessor', selectedAccessor);
    fixture.detectChanges();
  });

  it('should render root nodes fetching on init', () => {
    const treeText = fixture.nativeElement.textContent;
    expect(treeText).toContain('Folder 1');
    expect(treeText).toContain('File 2');
    expect(fetchDataMock).toHaveBeenCalledWith(null, 0, 50);
  });

  it('should emit clickChange with node data when a leaf node is clicked', () => {
    const clickSpy = vi.spyOn(component.clickChange, 'emit');

    const leafNodes = fixture.debugElement.queryAll(By.css('.leaf-node'));
    const file2Node = leafNodes.find((n) => n.nativeElement.textContent.includes('File 2'));

    expect(file2Node).toBeTruthy();
    file2Node!.nativeElement.click();

    expect(clickSpy).toHaveBeenCalledWith(mockData[1]);
    expect(clickSpy.mock.calls[0][0].data?.tags).toContain('readonly');
  });

  it('should fetch children when a folder is toggled', () => {
    fetchDataMock.mockReturnValueOnce(of({ data: [{ data: { id: '1.1', name: 'File 1.1', hasChildren: false, tags: ['child'] } }], totalCount: 1 }));

    const toggleButton = fixture.debugElement.query(By.css('button[matTreeNodeToggle]'));
    toggleButton.nativeElement.click();
    fixture.detectChanges();

    expect(fetchDataMock).toHaveBeenCalledWith('1', 0, 50);
    const treeText = fixture.nativeElement.textContent;
    expect(treeText).toContain('File 1.1');
  });

  it('should render an error state node when fetching fails', () => {
    fetchDataMock.mockReturnValueOnce(throwError(() => new HttpErrorResponse({ error: { title: 'Failed' } })));

    const toggleButton = fixture.debugElement.query(By.css('button[matTreeNodeToggle]'));
    toggleButton.nativeElement.click();
    fixture.detectChanges();

    const virtualError = fixture.debugElement.query(By.css('.virtual-node mat-icon.mat-warn'));
    expect(virtualError).toBeTruthy();
    expect(fixture.nativeElement.textContent).toContain('Error loading folder');
  });

  it('should render a load-more button when totalCount is greater than current data length', () => {
    fetchDataMock.mockReturnValueOnce(of({ data: [{ data: { id: '1.1', name: 'File 1.1', hasChildren: false, tags: ['child'] } }], totalCount: 5 }));

    const toggleButton = fixture.debugElement.query(By.css('button[matTreeNodeToggle]'));
    toggleButton.nativeElement.click();
    fixture.detectChanges();

    const allButtons = Array.from(fixture.nativeElement.querySelectorAll('button')) as HTMLButtonElement[];
    const loadMoreBtn = allButtons.find((b) => b.textContent?.includes('Load More'));

    expect(loadMoreBtn).toBeTruthy();
  });

  describe('Imperative API', () => {
    it('should update a node name using updateNode', () => {
      fixture.detectChanges();

      component.updateNode('2', (old) => ({ ...old, data: { ...old.data, name: 'Updated Name' } }));
      fixture.detectChanges();

      expect(fixture.nativeElement.textContent).toContain('Updated Name');
      expect(fixture.nativeElement.textContent).not.toContain('File 2');
    });

    it('should remove a node using removeNode', () => {
      fixture.detectChanges();

      component.removeNode('2');
      fixture.detectChanges();

      expect(fixture.nativeElement.textContent).not.toContain('File 2');
    });

    it('should reload the entire tree using resetTree', () => {
      fetchDataMock.mockClear();
      component.resetTree();
      fixture.detectChanges();

      expect(fetchDataMock).toHaveBeenCalledWith(null, 0, 50);
    });

    it('should reload a specific folder using reloadFolder', () => {
      fetchDataMock.mockClear();

      component.reloadFolder(null);

      expect(fetchDataMock).toHaveBeenCalledWith(null, 0, 50);
    });
  });
});
