import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PluginsMarketplaceComponent } from './plugins-marketplace.component';

describe('PluginsMarketplaceComponent', () => {
  let component: PluginsMarketplaceComponent;
  let fixture: ComponentFixture<PluginsMarketplaceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PluginsMarketplaceComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PluginsMarketplaceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
