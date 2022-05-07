import {Component} from '@angular/core';
import {Router} from '@angular/router';
import {debounceTime} from 'rxjs/operators';
import {ThemeService} from './services/theme/theme.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {
  title = 'app';

  constructor(private router: Router, private themeService: ThemeService) {
    this.router.events.pipe(debounceTime(10))
      .subscribe(() => {
        themeService.update();
      });
  }
}
