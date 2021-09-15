import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ProjectService } from '../services/project.service';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html'
})
export class FetchDataComponent {
  public forecasts: WeatherForecast[];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string, private _projectService: ProjectService) {
    http.get<WeatherForecast[]>(baseUrl + 'api/weatherforecast').subscribe(result => {
      this.forecasts = result;
    }, error => console.error(error));
  }

  doSmth() {
    this._projectService.getProject("id001").subscribe(ceva => { console.log(ceva); });
  }
}

interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}
