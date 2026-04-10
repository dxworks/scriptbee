import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RenameFileDialog, InstanceNotAllocatedDialogData } from './rename-file-dialog.component';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { By } from '@angular/platform-browser';
import { describe, it, expect, vi, beforeEach } from 'vitest';

describe('RenameFileDialog', () => {
  let component: RenameFileDialog;
  let fixture: ComponentFixture<RenameFileDialog>;
  let dialogRefSpy: { close: ReturnType<typeof vi.fn> };

  const mockData: InstanceNotAllocatedDialogData = {
    projectId: 'test-project',
    scriptId: 'test-script',
    currentScriptName: 'initial-name',
  };

  beforeEach(async () => {
    dialogRefSpy = { close: vi.fn() };

    await TestBed.configureTestingModule({
      imports: [RenameFileDialog],
      providers: [
        { provide: MAT_DIALOG_DATA, useValue: mockData },
        { provide: MatDialogRef, useValue: dialogRefSpy },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(RenameFileDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should display the current script name in the title', () => {
    const titleElement = fixture.debugElement.query(By.css('h1')).nativeElement;

    expect(titleElement.textContent).toContain('initial-name');
  });

  it('should initialize the input with the current script name', async () => {
    await fixture.whenStable();
    const inputElement = fixture.debugElement.query(By.css('input')).nativeElement as HTMLInputElement;

    expect(inputElement.value).toBe('initial-name');
  });

  it('should update the name when typing in the input', async () => {
    const inputDebug = fixture.debugElement.query(By.css('input'));
    const inputElement = inputDebug.nativeElement as HTMLInputElement;

    inputElement.value = 'updated-name';
    inputElement.dispatchEvent(new Event('input'));
    fixture.detectChanges();

    expect(component.newName()).toBe('updated-name');
  });

  it('should close the dialog with the new name when clicking RENAME', () => {
    const renameButton = fixture.debugElement.queryAll(By.css('button'))[0];

    component.newName.set('new-script-name');
    fixture.detectChanges();

    renameButton.nativeElement.click();

    expect(dialogRefSpy.close).toHaveBeenCalledWith('new-script-name');
  });

  it('should close the dialog when clicking CLOSE', () => {
    const closeButton = fixture.debugElement.queryAll(By.css('button'))[1];
    closeButton.nativeElement.click();

    expect(dialogRefSpy.close).toHaveBeenCalled();
  });
});
