import { inject, Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';

@Pipe({
  name: 'safeUrl',
})
export class SafeUrlPipe implements PipeTransform {
  private domSanitizer = inject(DomSanitizer);

  transform(value: string) {
    return this.domSanitizer.bypassSecurityTrustUrl(value);
  }
}
