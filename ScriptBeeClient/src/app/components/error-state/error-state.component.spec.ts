import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ErrorStateComponent } from './error-state.component';
import { ErrorResponse } from '../../types/api';
import { By } from '@angular/platform-browser';
import { Clipboard } from '@angular/cdk/clipboard';
import { MatSnackBar } from '@angular/material/snack-bar';
import { describe, it, expect, beforeEach, vi, Mock } from 'vitest';

describe('ErrorStateComponent', () => {
  let component: ErrorStateComponent;
  let fixture: ComponentFixture<ErrorStateComponent>;
  let clipboardSpy: { copy: Mock };
  let snackBarSpy: { open: Mock };

  const mockError: ErrorResponse = {
    title: 'Test Error',
    status: 500,
    detail: 'This is a test error detail',
    errors: {
      Field1: ['Message 1', 'Message 2'],
    },
  };

  beforeEach(async () => {
    clipboardSpy = { copy: vi.fn() };
    snackBarSpy = { open: vi.fn() };

    await TestBed.configureTestingModule({
      imports: [ErrorStateComponent],
    })
      .overrideComponent(ErrorStateComponent, {
        add: {
          providers: [
            { provide: Clipboard, useValue: clipboardSpy },
            { provide: MatSnackBar, useValue: snackBarSpy },
          ],
        },
      })
      .compileComponents();

    fixture = TestBed.createComponent(ErrorStateComponent);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('error', mockError);
    fixture.detectChanges();
  });

  it('should display error title', () => {
    const title = fixture.debugElement.query(By.css('mat-card-title')).nativeElement;

    expect(title.textContent).toContain('Test Error');
  });

  it('should display the HTTP status code in the subtitle', () => {
    const subtitle = fixture.debugElement.query(By.css('mat-card-subtitle')).nativeElement;

    expect(subtitle.textContent).toContain('500');
  });

  it('should display error detail message', () => {
    const detailEl = fixture.debugElement.query(By.css('mat-card-content p')).nativeElement;

    expect(detailEl.textContent).toContain('This is a test error detail');
  });

  it('should display validation errors inside expansion panel when present', () => {
    const panelHeader = fixture.debugElement.query(By.css('mat-expansion-panel-header')).nativeElement;
    panelHeader.click();
    fixture.detectChanges();

    const listItems = fixture.debugElement.queryAll(By.css('mat-list-item'));
    expect(listItems.length).toBeGreaterThan(0);

    const listText = fixture.debugElement.query(By.css('mat-list')).nativeElement.textContent;
    expect(listText).toContain('Field1');
    expect(listText).toContain('Message 1');
    expect(listText).toContain('Message 2');
  });

  it('should emit retry event when retry button is clicked', () => {
    const retrySpy = vi.fn();
    component.retry.subscribe(retrySpy);

    const buttons = fixture.debugElement.queryAll(By.css('mat-card-actions button'));
    const retryButton = buttons.find((b) => b.nativeElement.textContent.includes('Retry'))!.nativeElement;
    retryButton.click();

    expect(retrySpy).toHaveBeenCalled();
  });

  it('should copy support info and show snackbar when copy button is clicked', () => {
    const copyButton = fixture.debugElement.query(By.css('.copy-button'));
    copyButton.triggerEventHandler('click', null);
    fixture.detectChanges();

    expect(clipboardSpy.copy).toHaveBeenCalledWith(JSON.stringify(mockError, null, 2));
    expect(snackBarSpy.open).toHaveBeenCalled();
  });
});
