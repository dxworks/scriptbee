import { beforeEach, describe, expect, it, vi } from 'vitest';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CheckableTreeComponent } from './checkable-tree.component';
import { TreeNode } from '../../../types/tree-node';
import { MatTreeModule } from '@angular/material/tree';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { By } from '@angular/platform-browser';

interface TestData {
  id: string;
  name: string;
}

describe('CheckableTreeComponent', () => {
  let component: CheckableTreeComponent<TestData>;
  let fixture: ComponentFixture<CheckableTreeComponent<TestData>>;

  const mockData: TreeNode<TestData>[] = [
    {
      data: { id: 'f1', name: 'Folder 1' },
      children: [{ data: { id: 'file1.1', name: 'File 1.1' } }, { data: { id: 'file1.2', name: 'File 1.2' } }],
    },
    { data: { id: 'file2', name: 'File 2' } },
  ];

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CheckableTreeComponent, MatTreeModule, MatCheckboxModule, MatButtonModule, MatIconModule],
    }).compileComponents();

    fixture = TestBed.createComponent<CheckableTreeComponent<TestData>>(CheckableTreeComponent);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('data', mockData);
    fixture.componentRef.setInput('displayNameAccessor', (node: TreeNode<TestData>) => node.data.name);
    fixture.detectChanges();
  });

  it('should render nodes with checkboxes', () => {
    const checkboxes = fixture.debugElement.queryAll(By.css('mat-checkbox'));
    expect(checkboxes.length).toBeGreaterThanOrEqual(2);
  });

  it('should toggle selection when checkbox is clicked', () => {
    const updateSpy = vi.spyOn(component.updateCheckedFiles, 'emit');

    const checkboxes = fixture.debugElement.queryAll(By.css('mat-checkbox'));
    const file2Checkbox = checkboxes.find((c) => c.nativeElement.textContent.includes('File 2'));

    expect(file2Checkbox).toBeTruthy();
    const input = file2Checkbox!.query(By.css('input')).nativeElement;
    input.click();
    fixture.detectChanges();

    expect(component.selection.isSelected(component.data()[1])).toBe(true);
    expect(updateSpy).toHaveBeenCalled();
    const emittedNodes = updateSpy.mock.calls[0][0] as TreeNode<TestData>[];
    expect(emittedNodes.some((n) => n.data?.id === 'file2')).toBe(true);
  });

  it('should select all children when parent is selected', async () => {
    const toggleButton = fixture.debugElement.query(By.css('button[matTreeNodeToggle]'));
    toggleButton.nativeElement.click();
    fixture.detectChanges();

    const checkboxes = fixture.debugElement.queryAll(By.css('mat-checkbox'));
    const folder1Checkbox = checkboxes.find((c) => c.nativeElement.textContent.includes('Folder 1'));

    expect(folder1Checkbox).toBeTruthy();
    folder1Checkbox!.query(By.css('input')).nativeElement.click();
    fixture.detectChanges();

    const folderNode = component.data()[0];
    expect(component.selection.isSelected(folderNode)).toBe(true);
    expect(component.selection.isSelected(folderNode.children![0])).toBe(true);
    expect(component.selection.isSelected(folderNode.children![1])).toBe(true);
  });

  it('should deselect all children when parent is deselected', () => {
    const folderNode = component.data()[0];
    component.selection.select(folderNode, ...folderNode.children!);

    const toggleButton = fixture.debugElement.query(By.css('button[matTreeNodeToggle]'));
    toggleButton.nativeElement.click();
    fixture.detectChanges();

    const checkboxes = fixture.debugElement.queryAll(By.css('mat-checkbox'));
    const folder1Checkbox = checkboxes.find((c) => c.nativeElement.textContent.includes('Folder 1'));

    folder1Checkbox!.query(By.css('input')).nativeElement.click();
    fixture.detectChanges();

    expect(component.selection.isSelected(folderNode)).toBe(false);
    expect(component.selection.isSelected(folderNode.children![0])).toBe(false);
    expect(component.selection.isSelected(folderNode.children![1])).toBe(false);
  });

  it('should show indeterminate state when some but not all children are selected', () => {
    const toggleButton = fixture.debugElement.query(By.css('button[matTreeNodeToggle]'));
    toggleButton.nativeElement.click();
    fixture.detectChanges();

    const checkboxes = fixture.debugElement.queryAll(By.css('mat-checkbox'));
    const file11Checkbox = checkboxes.find((c) => c.nativeElement.textContent.includes('File 1.1'));

    file11Checkbox!.query(By.css('input')).nativeElement.click();
    fixture.detectChanges();

    const folderNode = component.data()[0];
    expect(component.isIndeterminate(folderNode)).toBe(true);
    expect(component.selection.isSelected(folderNode)).toBe(false);
  });

  it('should select parent when all children are selected one by one', () => {
    const toggleButton = fixture.debugElement.query(By.css('button[matTreeNodeToggle]'));
    toggleButton.nativeElement.click();
    fixture.detectChanges();

    const checkboxes = fixture.debugElement.queryAll(By.css('mat-checkbox'));
    const file11Checkbox = checkboxes.find((c) => c.nativeElement.textContent.includes('File 1.1'));
    const file12Checkbox = checkboxes.find((c) => c.nativeElement.textContent.includes('File 1.2'));

    file11Checkbox!.query(By.css('input')).nativeElement.click();
    fixture.detectChanges();
    file12Checkbox!.query(By.css('input')).nativeElement.click();
    fixture.detectChanges();

    const folderNode = component.data()[0];
    expect(component.selection.isSelected(folderNode)).toBe(true);
    expect(component.isIndeterminate(folderNode)).toBe(false);
  });
});
