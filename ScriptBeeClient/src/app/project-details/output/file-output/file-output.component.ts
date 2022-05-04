import {Component, Input, OnInit} from '@angular/core';
import {OutputFile} from '../../../services/output/output-file';
import {OutputFilesService} from '../../../services/output/output-files.service';
import {HttpEventType, HttpResponse} from '@angular/common/http';

@Component({
  selector: 'app-file-output',
  templateUrl: './file-output.component.html',
  styleUrls: ['./file-output.component.scss']
})
export class FileOutputComponent implements OnInit {

  @Input()
  outputFiles: OutputFile[] = [];
  @Input()
  projectId = '';
  @Input()
  runId = '';

  constructor(private outputFilesService: OutputFilesService) {
  }

  ngOnInit(): void {
  }

  onDownloadFileButtonClick(file: OutputFile) {
    this.outputFilesService.downloadFile(file.filePath).subscribe((data) => {
      this.downloadFile(file.fileName, data);
    });
  }

  onDownloadAllButtonClick() {
    this.outputFilesService.downloadAll(this.projectId, this.runId).subscribe((data) => {
      this.downloadFile('outputFiles.zip', data);
    });
  }

  private downloadFile(fileName: string, data: any) {
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
