import { beforeEach, describe, expect, it, vi } from 'vitest';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SelectableTreeComponent } from './selectable-tree.component';
import { TreeAction, TreeNode } from '../../../types/tree-node';
import { MatTreeModule } from '@angular/material/tree';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { TreeActionsMenuComponent } from './tree-actions-menu/tree-actions-menu.component';
import { By } from '@angular/platform-browser';

interface TestData {
  id: string;
  description: string;
}

describe('SelectableTreeComponent', () => {
  let component: SelectableTreeComponent<TestData>;
  let fixture: ComponentFixture<SelectableTreeComponent<TestData>>;

  const mockData: TreeNode<TestData>[] = [
    {
      name: 'Folder 1',
      data: { id: 'f1', description: 'Folder 1 Description' },
      children: [
        { name: 'File 1.1', data: { id: 'file1.1', description: 'File 1.1 Description' } },
        { name: 'File 1.2', data: { id: 'file1.2', description: 'File 1.2 Description' } },
      ],
    },
    { name: 'File 2', data: { id: 'file2', description: 'File 2 Description' } },
  ];

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SelectableTreeComponent, MatTreeModule, MatCheckboxModule, MatButtonModule, MatIconModule, MatMenuModule, TreeActionsMenuComponent],
    }).compileComponents();

    fixture = TestBed.createComponent<SelectableTreeComponent<TestData>>(SelectableTreeComponent);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('data', mockData);
    fixture.detectChanges();
  });

  it('should render all root nodes', () => {
    const treeText = fixture.nativeElement.textContent;
    expect(treeText).toContain('Folder 1');
    expect(treeText).toContain('File 2');
  });

  it('should emit clickChange with node data when a leaf node is clicked', () => {
    const clickSpy = vi.spyOn(component.clickChange, 'emit');

    const leafNodes = fixture.debugElement.queryAll(By.css('.leaf-node'));
    const file2Node = leafNodes.find((n) => n.nativeElement.textContent.includes('File 2'));

    expect(file2Node).toBeTruthy();
    file2Node!.nativeElement.click();

    expect(clickSpy).toHaveBeenCalledWith(mockData[1]);
    expect(clickSpy.mock.calls[0][0].data?.id).toBe('file2');
  });

  it('should expand folder when toggle button is clicked', async () => {
    const toggleButton = fixture.debugElement.query(By.css('button[matTreeNodeToggle]'));
    expect(toggleButton).toBeTruthy();

    toggleButton.nativeElement.click();
    fixture.detectChanges();

    const treeText = fixture.nativeElement.textContent;
    expect(treeText).toContain('File 1.1');
    expect(treeText).toContain('File 1.2');
  });

  it('should show actions menu only when actions are provided', () => {
    const actions: TreeAction<TestData>[] = [{ label: 'Delete', icon: 'delete', callback: vi.fn() }];
    fixture.componentRef.setInput('actions', actions);
    fixture.detectChanges();

    const actionMenus = fixture.debugElement.queryAll(By.css('app-tree-actions-menu'));
    expect(actionMenus.length).toBeGreaterThan(0);
  });

  it('should call action callback with node data when action is clicked', async () => {
    const actionSpy = vi.fn();
    const actions: TreeAction<TestData>[] = [{ label: 'Delete', icon: 'delete', callback: actionSpy }];
    fixture.componentRef.setInput('actions', actions);
    fixture.detectChanges();

    const menuTrigger = fixture.debugElement.query(By.css('app-tree-actions-menu button'));
    menuTrigger.nativeElement.click();
    fixture.detectChanges();

    const menuButtons = document.querySelectorAll('.mat-mdc-menu-item');
    const deleteButton = Array.from(menuButtons).find((b) => b.textContent?.includes('Delete')) as HTMLButtonElement;

    expect(deleteButton).toBeTruthy();
    deleteButton.click();

    expect(actionSpy).toHaveBeenCalledWith(mockData[0]);
    expect(actionSpy.mock.calls[0][0].data?.id).toBe('f1');
  });
});
