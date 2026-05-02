import { ComponentFixture, TestBed } from '@angular/core/testing';
import { describe, it, expect, beforeEach, vi, Mock } from 'vitest';
import { By } from '@angular/platform-browser';
import { Component, NO_ERRORS_SCHEMA } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { of, throwError } from 'rxjs';
import { AnalysisFilePreviewDialogComponent, AnalysisFilePreviewDialogData } from './analysis-file-preview-dialog.component';
import { OutputFilesService } from '../../../services/analysis/output-files.service';

@Component({ template: '<div class="dummy-viewer"></div>' })
class DummyViewerComponent {}

describe('AnalysisFilePreviewDialogComponent', () => {
  let fixture: ComponentFixture<AnalysisFilePreviewDialogComponent>;
  let mockOutputFilesService: { downloadFile: Mock };

  const dialogData: AnalysisFilePreviewDialogData = {
    projectId: 'project-1',
    analysisId: 'analysis-1',
    file: { id: 'file-1', name: 'test.json', type: 'json' },
    pluginComponent: DummyViewerComponent,
  };

  beforeEach(async () => {
    mockOutputFilesService = {
      downloadFile: vi.fn().mockReturnValue(of(new Blob(['test content'], { type: 'text/plain' }))),
    };

    await TestBed.configureTestingModule({
      imports: [AnalysisFilePreviewDialogComponent],
      providers: [
        { provide: MatDialogRef, useValue: {} },
        { provide: MAT_DIALOG_DATA, useValue: dialogData },
        { provide: OutputFilesService, useValue: mockOutputFilesService },
      ],
      schemas: [NO_ERRORS_SCHEMA],
    }).compileComponents();

    fixture = TestBed.createComponent(AnalysisFilePreviewDialogComponent);
  });

  it('should show the file name in the title', () => {
    fixture.detectChanges();

    const title = fixture.debugElement.query(By.css('[mat-dialog-title]'));
    expect(title.nativeElement.textContent).toContain('test.json');
  });

  it('should show error state when download fails', async () => {
    mockOutputFilesService.downloadFile.mockReturnValue(throwError(() => new Error('Download failed')));

    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();

    const errorState = fixture.debugElement.query(By.css('app-error-state'));
    expect(errorState).toBeTruthy();
  });
});
