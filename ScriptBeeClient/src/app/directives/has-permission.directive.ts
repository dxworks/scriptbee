import { Directive, effect, inject, input, TemplateRef, ViewContainerRef } from '@angular/core';
import { PermissionsService } from '../services/auth/permissions.service';
import { Permission } from '../types/permissions';

@Directive({
  selector: '[hasPermission]',
})
export class HasPermissionDirective {
  hasPermission = input.required<Permission>();

  private templateRef = inject(TemplateRef);
  private viewContainer = inject(ViewContainerRef);
  private permissionService = inject(PermissionsService);
  private hasRenderedView = false;

  constructor() {
    effect(() => {
      const canRender = this.permissionService.hasPermission(this.hasPermission());

      if (canRender) {
        if (!this.hasRenderedView) {
          this.viewContainer.createEmbeddedView(this.templateRef);
          this.hasRenderedView = true;
        }
        return;
      }

      if (this.hasRenderedView) {
        this.viewContainer.clear();
        this.hasRenderedView = false;
      }
    });
  }
}
