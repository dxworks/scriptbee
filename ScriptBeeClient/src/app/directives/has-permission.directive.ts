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

  constructor() {
    effect(() => {
      if (this.permissionService.hasPermission(this.hasPermission())) {
        this.viewContainer.createEmbeddedView(this.templateRef);
      } else {
        this.viewContainer.clear();
      }
    });
  }
}
