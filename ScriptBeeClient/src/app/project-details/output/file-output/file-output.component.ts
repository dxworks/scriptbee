import { Component, Input } from '@angular/core';
import { OutputFile } from '../../../services/output/output-file';
import { OutputFilesService } from '../../../services/output/output-files.service';

@Component({
  selector: 'app-file-output',
  templateUrl: './file-output.component.html',
  styleUrls: ['./file-output.component.scss']
})
export class FileOutputComponent {

  @Input()
  outputFiles: OutputFile[] = [];
  @Input()
  projectId = '';
  @Input()
  runIndex: number = -1;

  constructor(private outputFilesService: OutputFilesService) {
  }

  onDownloadFileButtonClick(file: OutputFile) {
    this.outputFilesService.downloadFile(file.fileId, file.fileName).subscribe((data) => {
      FileOutputComponent.downloadFile(file.fileName, data);
    });
  }

  onDownloadAllButtonClick() {
    this.outputFilesService.downloadAll(this.projectId, this.runIndex).subscribe((data) => {
      FileOutputComponent.downloadFile('outputFiles.zip', data);
    });
  }

  // todo to be moved to common service
  private static downloadFile(fileName: string, data: any) {
    const a: any = document.createElement('a');
    document.body.appendChild(a);
    a.style = 'display: none';
    const blob = new Blob([data], {type: 'octet/stream'});
    const url = window.URL.createObjectURL(blob);
    a.href = url;
    a.download = fileName;
    a.click();
    window.URL.revokeObjectURL(url);
  }
}
