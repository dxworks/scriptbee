import {Component, OnInit} from '@angular/core';
import {TreeNode} from '../../shared/tree/tree.component';

@Component({
  selector: 'app-scripts-content',
  templateUrl: './scripts-content.component.html',
  styleUrls: ['./scripts-content.component.scss']
})
export class ScriptsContentComponent implements OnInit {

  demoTree: TreeNode[] = [
    {
      name: 'Fruit',
      children: [{name: 'Apple'}, {name: 'Banana'}, {name: 'Fruit loops'}],
    },
    {
      name: 'Vegetables',
      children: [
        {
          name: 'Green',
          children: [{name: 'Broccoli'}, {name: 'Brussels sprouts'}],
        },
        {
          name: 'Orange',
          children: [{name: 'Pumpkins'}, {name: 'Carrots'}],
        },
      ],
    },
  ];

  constructor() {
  }

  ngOnInit(): void {
  }

  setProjectId(id: string) {
  }
}
