import { describe, it, expect, beforeEach, vi, Mock } from 'vitest';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { ComponentRef, NO_ERRORS_SCHEMA } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { of } from 'rxjs';
import { FileOutputComponent } from './file-output.component';
import { OutputFilesService } from '../../../../../../services/analysis/output-files.service';
import { DownloadService } from '../../../../../../services/common/download.service';
import { FileViewerService, FileViewerPlugin } from '../../../../../../shared/file-viewer/file-viewer.service';
import { AnalysisFilePreviewDialogComponent } from '../../../../../../shared/file-viewer/analysis-file-preview-dialog/analysis-file-preview-dialog.component';
import { MonacoEditorViewerComponent } from '../../../../../../shared/file-viewer/monaco-editor-viewer/monaco-editor-viewer.component';

describe('FileOutputComponent', () => {
  let fixture: ComponentFixture<FileOutputComponent>;
  let componentRef: ComponentRef<FileOutputComponent>;
  let mockOutputFilesService: { downloadFile: Mock; downloadAll: Mock };
  let mockDownloadService: { downloadFile: Mock };
  let mockFileViewerService: { getAvailablePluginsForFile: Mock };
  let mockDialog: { open: Mock };

  const defaultPlugin: FileViewerPlugin = {
    id: 'monaco-editor-default',
    name: 'Monaco Editor',
    icon: 'code',
    component: MonacoEditorViewerComponent,
  };

  beforeEach(async () => {
    mockOutputFilesService = {
      downloadFile: vi.fn().mockReturnValue(of(new Blob())),
      downloadAll: vi.fn().mockReturnValue(of(new Blob())),
    };
    mockDownloadService = { downloadFile: vi.fn() };
    mockFileViewerService = {
      getAvailablePluginsForFile: vi.fn().mockReturnValue([defaultPlugin]),
    };
    mockDialog = { open: vi.fn() };

    await TestBed.configureTestingModule({
      imports: [FileOutputComponent],
      providers: [
        { provide: OutputFilesService, useValue: mockOutputFilesService },
        { provide: DownloadService, useValue: mockDownloadService },
        { provide: FileViewerService, useValue: mockFileViewerService },
        { provide: MatDialog, useValue: mockDialog },
      ],
      schemas: [NO_ERRORS_SCHEMA],
    }).compileComponents();

    fixture = TestBed.createComponent(FileOutputComponent);
    componentRef = fixture.componentRef;
    componentRef.setInput('projectId', 'proj-1');
    componentRef.setInput('analysisId', 'analysis-1');
    componentRef.setInput('files', []);
    fixture.detectChanges();
  });

  it('should show "no output files" message when files list is empty', () => {
    const message = fixture.debugElement.query(By.css('.no-output-files'));
    expect(message).toBeTruthy();
  });

  it('should render file items with names and icon buttons', () => {
    componentRef.setInput('files', [{ id: 'f1', name: 'report.json', type: 'json' }]);
    fixture.detectChanges();

    const fileName = fixture.debugElement.query(By.css('.file-name'));
    expect(fileName.nativeElement.textContent).toContain('report.json');

    const previewButton = fixture.debugElement.query(By.css('.file-preview-button'));
    expect(previewButton).toBeTruthy();
    expect(previewButton.nativeElement.getAttribute('title')).toBe('Preview');
  });

  it('should open preview dialog when clicking a file with a single viewer plugin', () => {
    componentRef.setInput('files', [{ id: 'f1', name: 'report.json', type: 'json' }]);
    fixture.detectChanges();

    const previewButton = fixture.debugElement.query(By.css('.file-preview-button'));
    previewButton.nativeElement.click();

    expect(mockDialog.open).toHaveBeenCalledWith(
      AnalysisFilePreviewDialogComponent,
      expect.objectContaining({
        data: expect.objectContaining({
          projectId: 'proj-1',
          analysisId: 'analysis-1',
          pluginComponent: MonacoEditorViewerComponent,
        }),
      })
    );
  });

  it('should trigger file download when clicking the download icon button', () => {
    componentRef.setInput('files', [{ id: 'f1', name: 'report.json', type: 'json' }]);
    fixture.detectChanges();

    const downloadButton = fixture.debugElement.query(By.css('.download-icon-button'));
    downloadButton.nativeElement.click();

    expect(mockOutputFilesService.downloadFile).toHaveBeenCalledWith('proj-1', 'analysis-1', 'f1');
    expect(mockDownloadService.downloadFile).toHaveBeenCalledWith('report.json', expect.any(Blob));
  });
});
