import { Component } from '@angular/core';
import { NoScriptsComponent } from './no-scripts/no-scripts.component';

@Component({
  selector: 'app-scripts-content',
  templateUrl: './scripts-content.component.html',
  styleUrls: ['./scripts-content.component.scss'],
  imports: [NoScriptsComponent],
})
export class ScriptsContentComponent {}
