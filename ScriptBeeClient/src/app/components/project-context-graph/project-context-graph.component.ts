import { AfterViewInit, Component, computed, effect, ElementRef, inject, input, OnDestroy, OnInit, signal, untracked, viewChild } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { ContextGraphEdge, ContextGraphNode } from '../../types/context-graph';
import { CytoscapeGraphController } from './cytoscape-graph-controller';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule, MatMenuTrigger } from '@angular/material/menu';
import { KeyValuePipe } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { ProjectContextService } from '../../services/projects/project-context.service';

@Component({
  selector: 'app-project-context-graph',
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    MatDividerModule,
    MatTooltipModule,
    MatProgressBarModule,
    MatSidenavModule,
    MatListModule,
    MatMenuModule,
    KeyValuePipe,
  ],
  templateUrl: './project-context-graph.component.html',
  styleUrls: ['./project-context-graph.component.scss'],
})
export class ProjectContextGraphComponent implements OnInit, AfterViewInit, OnDestroy {
  projectId = input.required<string>();
  instanceId = input.required<string>();

  container = viewChild.required<ElementRef>('container');
  contextMenuTrigger = viewChild.required<MatMenuTrigger>(MatMenuTrigger);

  searchControl = new FormControl('');
  selectedNode = signal<ContextGraphNode | null>(null);

  isLoading = signal<boolean>(false);
  error = signal<string | null>(null);
  hasMoreResults = signal<boolean>(false);
  isEmpty = signal<boolean>(true);

  menuPosition = { x: '0px', y: '0px' };

  private controller?: CytoscapeGraphController;
  private projectContextService = inject(ProjectContextService);
  private resizeObserver?: ResizeObserver;

  nodes = signal<ContextGraphNode[]>([]);
  edges: ContextGraphEdge[] = [];
  nodeCount = computed(() => this.nodes().length);

  private currentOffset = 0;
  private readonly currentLimit = 100;

  constructor() {
    effect(() => {
      this.projectId();
      this.instanceId();

      untracked(() => {
        this.controller?.clear();
        this.nodes.set([]);
        this.edges = [];
        this.currentOffset = 0;
        this.isEmpty.set(true);

        const query = this.searchControl.value;
        if (query) {
          this.performSearch(query);
        }
      });
    });
  }

  ngOnInit() {
    this.searchControl.valueChanges.pipe(debounceTime(300), distinctUntilChanged()).subscribe((query) => {
      this.nodes.set([]);
      this.edges = [];
      this.currentOffset = 0;
      this.controller?.clear();
      this.performSearch(query ?? '');
    });
  }

  loadMore() {
    this.currentOffset += this.currentLimit;
    this.performSearch(this.searchControl.value ?? '');
  }

  ngAfterViewInit() {
    this.controller = new CytoscapeGraphController(this.container().nativeElement);
    this.initControllerWhenReady();
  }

  ngOnDestroy() {
    this.controller?.kill();
    if (this.resizeObserver) {
      this.resizeObserver.disconnect();
    }
  }

  formatPropertyValue(value: unknown): string {
    if (value === null || value === undefined) {
      return '';
    }
    if (typeof value === 'object') {
      return JSON.stringify(value, null, 2);
    }
    return String(value);
  }

  expandNeighbors(nodeId: string) {
    this.isLoading.set(true);
    this.error.set(null);
    this.projectContextService.getNeighbors(this.projectId(), this.instanceId(), nodeId).subscribe({
      next: (result) => {
        this.isLoading.set(false);
        if (result && result.nodes) {
          this.addUniqueData(result.nodes, result.edges ?? []);
          this.controller?.updateData(result.nodes, result.edges ?? [], this.getNodeColor.bind(this));
        }
      },
      error: (err) => {
        this.isLoading.set(false);
        this.error.set(err.message || 'Failed to expand neighbors');
      },
    });
  }

  centerOnNode(nodeId: string) {
    this.controller?.cy?.animate({
      center: { eles: this.controller.cy.getElementById(nodeId) },
      zoom: 1.5,
      duration: 500,
    });
  }

  zoomIn() {
    this.controller?.zoomIn();
  }

  zoomOut() {
    this.controller?.zoomOut();
  }

  fitGraph() {
    this.controller?.center();
  }

  private performSearch(query: string) {
    if (!query || query.trim() === '') {
      this.hasMoreResults.set(false);
      this.isEmpty.set(true);
      return;
    }

    this.isLoading.set(true);
    this.error.set(null);

    this.projectContextService.searchNodes(this.projectId(), this.instanceId(), query, this.currentOffset, this.currentLimit).subscribe({
      next: (result) => {
        this.isLoading.set(false);
        if (result && result.nodes) {
          this.addUniqueData(result.nodes, result.edges ?? []);
          this.hasMoreResults.set(result.nodes.length === this.currentLimit);
          this.controller?.updateData(result.nodes, result.edges ?? [], this.getNodeColor.bind(this));
        }
      },
      error: (err) => {
        this.isLoading.set(false);
        this.error.set(err.message || 'Failed to search nodes');
      },
    });
  }

  private addUniqueData(newNodes: ContextGraphNode[], newEdges: ContextGraphEdge[]) {
    const currentNodes = this.nodes();
    const nodeMap = new Map<string, ContextGraphNode>();
    currentNodes.forEach((n) => nodeMap.set(n.id, n));
    newNodes.forEach((n) => nodeMap.set(n.id, n));

    this.nodes.set(Array.from(nodeMap.values()));
    this.edges = [...this.edges, ...newEdges];
    this.isEmpty.set(this.nodes().length === 0);
  }

  private initControllerWhenReady() {
    const el = this.container().nativeElement;

    this.resizeObserver = new ResizeObserver(() => {
      if (el.offsetWidth > 0 && el.offsetHeight > 0) {
        if (!this.controller?.isInitialized()) {
          this.initGraph();
        } else {
          this.controller?.refresh();
        }
      }
    });
    this.resizeObserver.observe(el);
  }

  private initGraph() {
    if (this.controller) {
      this.controller.init();
      this.setupHandlers();

      const currentNodes = this.nodes();
      if (currentNodes.length > 0) {
        this.controller.updateData(currentNodes, this.edges, this.getNodeColor.bind(this));
      } else {
        this.controller.refresh();
      }
    }
  }

  private setupHandlers() {
    this.controller?.onNodeClick((_, attributes) => {
      this.selectedNode.set(attributes);
    });

    this.controller?.onNodeHover();

    this.controller?.cy?.on('cxttap', 'node', (evt) => {
      const nodeId = evt.target.id();
      const originalEvent = evt.originalEvent;

      this.menuPosition = {
        x: `${originalEvent.clientX}px`,
        y: `${originalEvent.clientY}px`,
      };

      this.contextMenuTrigger().menuData = { nodeId };
      this.contextMenuTrigger().openMenu();
    });
  }

  private getNodeColor(type?: string): string {
    if (!type) return '#007bff';
    const hash = type.split('').reduce((acc, char) => char.charCodeAt(0) + ((acc << 5) - acc), 0);
    return `hsl(${Math.abs(hash % 360)}, 70%, 50%)`;
  }
}
