import {Component, ElementRef, ViewChild} from '@angular/core';
import {Upload} from './upload';
import * as _ from 'lodash';
import {UploadService} from '../../services/upload/upload.service';

@Component({
  selector: 'app-drag-and-drop-files',
  templateUrl: './drag-and-drop-files.component.html',
  styleUrls: ['./drag-and-drop-files.component.scss']
})
export class DragAndDropFilesComponent {

  @ViewChild('modelFileInput') modelFileInput: ElementRef;

  isHovering: boolean;
  files: any[] = [];

  toggleHover(event: boolean) {
    this.isHovering = event;
  }

  addFiles(files: FileList) {
    for (let i = 0; i < files.length; i++) {
      this.files.push(files.item(i));
    }
    console.log(files);
  }

  openBrowseFilesDialog() {
    this.modelFileInput.nativeElement.value = "";
    this.modelFileInput.nativeElement.click();
  }

  onClearButtonClick(index) {
    this.files.splice(index, 1);
  }
}
