import { describe, it, expect } from 'vitest';
import { TestBed } from '@angular/core/testing';
import { FileViewerService } from './file-viewer.service';
import { AnalysisFile } from '../../types/analysis-results';
import { MonacoEditorViewerComponent } from './monaco-editor-viewer/monaco-editor-viewer.component';

describe('FileViewerService', () => {
  let service: FileViewerService;

  const jsonFile: AnalysisFile = { id: '1', name: 'report.json', type: 'json' };

  it('should provide the monaco editor as the default previewer for a file', () => {
    TestBed.configureTestingModule({ providers: [FileViewerService] });
    service = TestBed.inject(FileViewerService);

    const previewers = service.getAvailablePreviewersForFile(jsonFile);
    expect(previewers.length).toBeGreaterThan(0);
    expect(previewers[0].id).toBe('monaco-editor-default');
    expect(previewers[0].component).toBe(MonacoEditorViewerComponent);
  });
});
