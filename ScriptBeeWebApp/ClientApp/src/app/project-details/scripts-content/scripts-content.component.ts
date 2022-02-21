import {Component, OnInit} from '@angular/core';
import {FileSystemService} from "../../services/file-system/file-system.service";
import {ProjectDetailsService} from "../project-details.service";
import {FileTreeNode} from "./fileTreeNode";
import {ActivatedRoute, Router} from "@angular/router";

@Component({
  selector: 'app-scripts-content',
  templateUrl: './scripts-content.component.html',
  styleUrls: ['./scripts-content.component.scss']
})
export class ScriptsContentComponent implements OnInit {

  fileStructureTree: FileTreeNode[] = [];

  constructor(private fileSystemService: FileSystemService, private projectDetailsService: ProjectDetailsService, private router: Router, private route: ActivatedRoute) {
  }

  ngOnInit(): void {
    this.projectDetailsService.project.subscribe(project => {
      if (project) {
        this.fileSystemService.getFileSystem(project.projectId).subscribe(fileTreeNode => {
          if (fileTreeNode) {
            this.fileStructureTree = [fileTreeNode];
          }
        });
      }
    })
  }

  onLeafClick(node: FileTreeNode) {
    this.router.navigate([node.srcPath], {relativeTo: this.route});
  }

  onCreateNewProjectButtonClick() {

  }
}
