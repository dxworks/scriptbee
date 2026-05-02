import { describe, expect, it } from 'vitest';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Component, ComponentRef, forwardRef, input, signal } from '@angular/core';
import { MonacoEditorViewerComponent } from './monaco-editor-viewer.component';
import { AnalysisFile } from '../../../types/analysis-results';
import { ThemeService } from '../../../services/common/theme.service';
import { EditorComponent } from 'ngx-monaco-editor-v2';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
  selector: 'ngx-monaco-editor',
  template: '',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => MockEditorComponent),
      multi: true,
    },
  ],
})
class MockEditorComponent implements ControlValueAccessor {
  options = input<EditorComponent['options']>();

  writeValue(): void {
    // no-op since this is a mock component for testing purposes
  }
  registerOnChange(): void {
    // no-op since this is a mock component for testing purposes
  }
  registerOnTouched(): void {
    // no-op since this is a mock component for testing purposes
  }
}

describe('MonacoEditorViewerComponent', () => {
  async function createFixture(file: AnalysisFile): Promise<ComponentFixture<MonacoEditorViewerComponent>> {
    await TestBed.configureTestingModule({
      imports: [MonacoEditorViewerComponent],
      providers: [{ provide: ThemeService, useValue: { darkMode: signal(false) } }],
    })
      .overrideComponent(MonacoEditorViewerComponent, {
        remove: { imports: [EditorComponent] },
        add: { imports: [MockEditorComponent] },
      })
      .compileComponents();

    const fixture = TestBed.createComponent(MonacoEditorViewerComponent);
    const componentRef: ComponentRef<MonacoEditorViewerComponent> = fixture.componentRef;
    componentRef.setInput('content', 'sample content');
    componentRef.setInput('file', file);
    fixture.detectChanges();
    return fixture;
  }

  it('should set json language for .json files', async () => {
    const fixture = await createFixture({ id: '1', name: 'report.json', type: 'json' });
    expect(fixture.componentInstance.editorOptions().language).toBe('json');
  });

  it('should set xml language for .xml files', async () => {
    const fixture = await createFixture({ id: '2', name: 'data.xml', type: 'xml' });
    expect(fixture.componentInstance.editorOptions().language).toBe('xml');
  });

  it('should set javascript language for .js files', async () => {
    const fixture = await createFixture({ id: '3', name: 'script.js', type: 'js' });
    expect(fixture.componentInstance.editorOptions().language).toBe('javascript');
  });

  it('should default to plaintext for unknown extensions', async () => {
    const fixture = await createFixture({ id: '4', name: 'data.unknown', type: 'unknown' });
    expect(fixture.componentInstance.editorOptions().language).toBe('plaintext');
  });

  it('should switch theme based on darkMode signal', async () => {
    const darkMode = signal(false);
    await TestBed.configureTestingModule({
      imports: [MonacoEditorViewerComponent],
      providers: [{ provide: ThemeService, useValue: { darkMode } }],
    })
      .overrideComponent(MonacoEditorViewerComponent, {
        remove: { imports: [EditorComponent] },
        add: { imports: [MockEditorComponent] },
      })
      .compileComponents();

    const fixture = TestBed.createComponent(MonacoEditorViewerComponent);
    fixture.componentRef.setInput('content', 'sample content');
    fixture.componentRef.setInput('file', { id: '1', name: 'report.json', type: 'json' });
    fixture.detectChanges();

    expect(fixture.componentInstance.editorOptions().theme).toBe('vs-light');

    darkMode.set(true);
    fixture.detectChanges();

    expect(fixture.componentInstance.editorOptions().theme).toBe('vs-dark');
  });
});
