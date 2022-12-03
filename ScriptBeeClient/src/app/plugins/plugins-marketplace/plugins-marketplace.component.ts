import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { MatSort } from "@angular/material/sort";
import { MatPaginator } from "@angular/material/paginator";
import { MatTableDataSource } from '@angular/material/table';
import { PluginService } from "../../services/plugin/plugin.service";
import { animate, state, style, transition, trigger } from "@angular/animations";
import { MarketplacePlugin } from "../../services/plugin/marketplace-plugin";

@Component({
  selector: 'app-plugins-marketplace',
  templateUrl: './plugins-marketplace.component.html',
  styleUrls: ['./plugins-marketplace.component.scss'],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({ height: '0px', minHeight: '0' })),
      state('expanded', style({ height: '*' })),
      transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
  ],
})
export class PluginsMarketplaceComponent implements OnInit, AfterViewInit {
  displayedColumns: string[] = ['name', 'author', 'expand'];
  dataSource: MatTableDataSource<MarketplacePlugin>;
  expandedElement: MarketplacePlugin | null;

  isLoading = false;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(private pluginService: PluginService) {
    this.dataSource = new MatTableDataSource([]);
  }

  ngOnInit() {
    this.isLoading = true;
    this.pluginService.getAllAvailablePlugins().subscribe({
      next: plugins => {
        this.isLoading = false;
        this.dataSource = new MatTableDataSource<MarketplacePlugin>(plugins);
      }, error: () => {
        this.isLoading = false;
        // todo display error
      }
    });
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }
}
