import {Component, OnInit} from '@angular/core';
import {ThemeService} from '../../../services/theme/theme.service';
import {FileSystemService} from "../../../services/file-system/file-system.service";
import {ProjectDetailsService} from "../../project-details.service";
import {ActivatedRoute} from "@angular/router";

@Component({
  selector: 'app-selected-script',
  templateUrl: './selected-script.component.html',
  styleUrls: ['./selected-script.component.scss']
})
export class SelectedScriptComponent implements OnInit {

  editorOptions = {theme: 'vs-dark', language: 'javascript', readOnly: true};
  code = '';

  constructor(private themeService: ThemeService, private fileSystemService: FileSystemService,
              private projectDetailsService: ProjectDetailsService, private route: ActivatedRoute) {
  }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      if (params) {
        const scriptPath = params["scriptPath"];

        this.setEditorLanguage(scriptPath)

        this.projectDetailsService.project.subscribe(project => {
          if (project) {
            this.fileSystemService.getFileContent(project.projectId, scriptPath).subscribe(content => {
              this.code = content;
            })
          }
        });
      }
    });

    this.themeService.darkThemeSubject.subscribe(isDark => {
      if (isDark) {
        this.editorOptions = {...this.editorOptions, theme: 'vs-dark'};
      } else {
        this.editorOptions = {...this.editorOptions, theme: 'vs-light'};
      }
    });
  }

  onRunScriptButtonClick() {

  }

  private setEditorLanguage(scriptPath: string) {
    if (!scriptPath) {
      return;
    }

    if (scriptPath.endsWith(".js")) {
      this.editorOptions = {...this.editorOptions, language: 'javascript'};
    } else if (scriptPath.endsWith(".py")) {
      this.editorOptions = {...this.editorOptions, language: 'python'};
    } else if (scriptPath.endsWith(".cs")) {
      this.editorOptions = {...this.editorOptions, language: 'csharp'};
    }
  }
}
