import { describe, expect, it, vi, beforeEach } from 'vitest';
import { TestBed } from '@angular/core/testing';
import { Component, input } from '@angular/core';
import { RemotePluginHostComponent } from './remote-plugin-host.component';

const mockLoadRemoteModule = vi.fn();

vi.mock('@angular-architects/native-federation', () => ({
  loadRemoteModule: (...args: unknown[]) => mockLoadRemoteModule(...args),
}));

@Component({
  selector: 'app-test-remote',
  template: '<div>Plugin Content Successfully Loaded!</div>',
  standalone: true,
})
class TestRemoteComponent {
  inputs = input<unknown>();
}

describe('RemotePluginHostComponent', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RemotePluginHostComponent, TestRemoteComponent],
    }).compileComponents();

    mockLoadRemoteModule.mockReset();
  });

  it('should render the remote component content on success', async () => {
    mockLoadRemoteModule.mockResolvedValue({ App: TestRemoteComponent });
    const fixture = TestBed.createComponent(RemotePluginHostComponent);

    const plugin = {
      remoteName: 'test',
      exposedModule: './Component',
      componentName: 'App',
    };
    fixture.componentRef.setInput('pluginOutlet', plugin);

    fixture.detectChanges();
    await new Promise((resolve) => setTimeout(resolve));
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Plugin Content Successfully Loaded!');
  });

  it('should display an error message if the plugin fails to load', async () => {
    mockLoadRemoteModule.mockRejectedValue(new Error('Network error'));
    const fixture = TestBed.createComponent(RemotePluginHostComponent);

    const plugin = {
      remoteName: 'test',
      exposedModule: './Component',
      componentName: 'App',
    };
    fixture.componentRef.setInput('pluginOutlet', plugin);

    fixture.detectChanges();
    await new Promise((resolve) => setTimeout(resolve));
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Failed to load plugin');
  });
});
