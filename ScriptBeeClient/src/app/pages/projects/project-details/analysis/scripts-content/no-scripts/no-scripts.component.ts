import { Component, OnInit } from '@angular/core';
import { MatMiniFabButton } from '@angular/material/button';

type Action = {
  name: string;
  icon: string;
  color: string;
};

@Component({
  selector: 'app-no-scripts',
  templateUrl: './no-scripts.component.html',
  styleUrls: ['./no-scripts.component.scss'],
  imports: [MatMiniFabButton],
})
export class NoScriptsComponent implements OnInit {
  actions: Action[] = [];

  ngOnInit(): void {
    this.actions = [
      {
        name: 'Create a new script using the button on the left.',
        icon: 'looks_one',
        color: '#9C27B0',
      },
      { name: 'Find the script in the file tree and select it.', icon: 'looks_two', color: '#673AB7' },
      {
        name: 'Open the script in Visual Studio Code using the logo button from the top right corner.',
        icon: 'looks_3',
        color: '#FF9800',
      },
      { name: 'Edit the script in Visual Studio Code and save the changes.', icon: 'looks_4', color: '#607D8B' },
      {
        name: 'Return to ScriptBee to run the script using the arrow button from the top right corner.',
        icon: 'looks_5',
        color: '#607D8B',
      },
      {
        name: 'Check the results that will be displayed in the output tabs located at the bottom of the page.',
        icon: 'looks_6',
        color: '#607D8B',
      },
    ];
  }
}
