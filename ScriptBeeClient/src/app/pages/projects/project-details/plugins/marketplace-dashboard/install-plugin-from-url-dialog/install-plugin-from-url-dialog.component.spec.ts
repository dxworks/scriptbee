import { ComponentFixture, TestBed } from '@angular/core/testing';
import { InstallPluginFromUrlDialogComponent } from './install-plugin-from-url-dialog.component';
import { MatDialogRef } from '@angular/material/dialog';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { By } from '@angular/platform-browser';

describe('InstallPluginFromUrlDialogComponent', () => {
  let component: InstallPluginFromUrlDialogComponent;
  let fixture: ComponentFixture<InstallPluginFromUrlDialogComponent>;
  let dialogRefSpy: { close: unknown };

  beforeEach(async () => {
    dialogRefSpy = { close: vi.fn() };

    await TestBed.configureTestingModule({
      imports: [InstallPluginFromUrlDialogComponent],
      providers: [{ provide: MatDialogRef, useValue: dialogRefSpy }],
    }).compileComponents();

    fixture = TestBed.createComponent(InstallPluginFromUrlDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should not close with URL if input is empty or whitespace', () => {
    component.url.set('   ');
    fixture.detectChanges();

    const installButton = fixture.debugElement.query(By.css('button[color="primary"]')).nativeElement as HTMLButtonElement;
    expect(installButton.disabled).toBe(true);

    component.confirm();
    expect(dialogRefSpy.close).not.toHaveBeenCalled();
  });

  it('should close dialog with trimmed URL when confirm is called with non-empty input', () => {
    component.url.set('  https://example.com/plugin.zip  ');
    fixture.detectChanges();

    const installButton = fixture.debugElement.query(By.css('button[color="primary"]')).nativeElement as HTMLButtonElement;
    expect(installButton.disabled).toBe(false);

    installButton.click();
    expect(dialogRefSpy.close).toHaveBeenCalledWith('https://example.com/plugin.zip');
  });
});
