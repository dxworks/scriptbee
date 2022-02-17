import {Component, OnInit} from '@angular/core';
import {ThemeService} from '../../../services/theme/theme.service';

@Component({
  selector: 'app-selected-script',
  templateUrl: './selected-script.component.html',
  styleUrls: ['./selected-script.component.scss']
})
export class SelectedScriptComponent implements OnInit {

  editorOptions = {theme: 'vs-dark', language: 'javascript', readOnly: true};
  code = 'function x() {\nconsole.log("Hello world!");\n}';
  originalCode = 'function x() { // TODO }';

  constructor(private themeService: ThemeService) {
  }

  ngOnInit(): void {
    this.themeService.darkThemeSubject.subscribe(isDark => {
      if (isDark) {
        this.editorOptions = {...this.editorOptions, theme: 'vs-dark'};
      } else {
        this.editorOptions = {...this.editorOptions, theme: 'vs-light'};
      }
    });
  }
}
