import { PluginMarketplaceDashboardListComponent } from './plugin-marketplace-dashboard-list.component';
import { createComponentFactory } from '@ngneat/spectator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { createMarketplacePlugin } from '../../../../../test/marketplacePluginUtils';
import { ApiErrorMessageComponent } from '../../../shared/api-error-message/api-error-message.component';
import { MockComponents } from 'ng-mocks';
import { PluginMarketplaceDashboardListItemComponent } from '../plugin-marketplace-dashboard-list-item/plugin-marketplace-dashboard-list-item.component';
import { By } from '@angular/platform-browser';

describe('PluginMarketplaceDashboardListComponent', () => {
  const createComponent = createComponentFactory({
    component: PluginMarketplaceDashboardListComponent,
    declarations: [...MockComponents(PluginMarketplaceDashboardListItemComponent, ApiErrorMessageComponent)],
    imports: [MatProgressSpinnerModule]
  });

  it('should create', () => {
    const component = createComponent();
    expect(component).toBeTruthy();
  });

  it('given empty plugins, then no plugins available message is shown', () => {
    const component = createComponent({ props: { plugins: [], error: undefined } });

    const noDataMessageComponent = component.debugElement.query(By.directive(ApiErrorMessageComponent));

    expect(noDataMessageComponent.componentInstance.type).toEqual('no-data');
    expect(noDataMessageComponent.componentInstance.message).toEqual('No plugins available');
  });

  it('given plugins and error, then error message is shown', () => {
    const component = createComponent({
      props: {
        plugins: [createMarketplacePlugin('1', '2')],
        error: { message: 'error', code: 400 }
      }
    });

    const errorMessageComponent = component.debugElement.query(By.directive(ApiErrorMessageComponent));

    expect(errorMessageComponent.componentInstance.type).toEqual('error');
    expect(errorMessageComponent.componentInstance.message).toEqual('An error occurred while loading plugins');
  });

  it('given plugins, then plugins are shown', () => {
    const component = createComponent({
      props: {
        plugins: [
          createMarketplacePlugin('1', 'plugin-name1'),
          createMarketplacePlugin('2', 'plugin-name2'),
          createMarketplacePlugin('3', 'plugin-name3')
        ],
        error: undefined
      }
    });

    const items = component.debugElement.queryAll(By.directive(PluginMarketplaceDashboardListItemComponent));

    expect(items.length).toBe(3);
  });

  describe('loading plugins spinner', () => {
    it('given undefined plugins and undefined error, then should show spinner when loading', () => {
      const component = createComponent({ props: { plugins: undefined, error: undefined } });

      const spinner = component.query('mat-spinner');

      expect(spinner).toBeTruthy();
    });

    it('given undefined plugins and error, then should not show spinner when not loading', () => {
      const component = createComponent({ props: { plugins: undefined, error: { message: 'error', code: 500 } } });

      const spinner = component.query('mat-spinner');

      expect(spinner).toBeFalsy();
    });

    it('given plugins and undefined error, then should not show spinner when not loading', () => {
      const component = createComponent({ props: { plugins: [createMarketplacePlugin('1', '2')], error: undefined } });

      const spinner = component.query('mat-spinner');

      expect(spinner).toBeFalsy();
    });

    it('given plugins and error, then should not show spinner when not loading', () => {
      const component = createComponent({
        props: {
          plugins: [createMarketplacePlugin('1', '2')],
          error: { message: 'error', code: 400 }
        }
      });

      const spinner = component.query('mat-spinner');

      expect(spinner).toBeFalsy();
    });
  });
});
