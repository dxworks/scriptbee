import cytoscape from 'cytoscape';
import { ContextGraphEdge, ContextGraphNode } from '../../types/context-graph';

export class CytoscapeGraphController {
  cy?: cytoscape.Core;

  constructor(private container: HTMLElement) {}

  isInitialized(): boolean {
    return !!this.cy;
  }

  init() {
    if (this.cy) {
      return;
    }

    this.cy = cytoscape({
      container: this.container,
      wheelSensitivity: 5,
      elements: [],
      style: [
        {
          selector: 'node',
          style: {
            'background-color': 'data(color)',
            label: 'data(displayLabel)',
            color: '#333',
            'text-valign': 'center',
            'text-halign': 'center',
            'font-size': '12px',
            'text-outline-color': '#fff',
            'text-outline-width': 2,
            width: 40,
            height: 40,
          },
        },
        {
          selector: 'edge',
          style: {
            width: 2,
            'line-color': '#ccc',
            'target-arrow-color': '#ccc',
            'target-arrow-shape': 'triangle',
            'curve-style': 'bezier',
            label: 'data(label)',
            'font-size': '10px',
            'text-rotation': 'autorotate',
            'text-background-opacity': 1,
            'text-background-color': '#fff',
            'text-background-padding': '2',
          },
        },
      ],
      layout: {
        name: 'cose',
        animate: false,
      },
    });
  }

  updateData(nodes: ContextGraphNode[], edges: ContextGraphEdge[], nodeColorProvider: (type: string) => string) {
    if (!this.cy) {
      return;
    }

    const newElements: cytoscape.ElementDefinition[] = [];
    const addedIds = new Set<string>();

    nodes.forEach((node) => {
      if (!node || !node.id) {
        return;
      }

      if (this.cy!.getElementById(node.id).length === 0 && !addedIds.has(node.id)) {
        addedIds.add(node.id);
        const fullLabel = node.label || node.id;
        newElements.push({
          group: 'nodes',
          data: {
            id: node.id,
            label: fullLabel,
            displayLabel: this.truncateLabel(fullLabel),
            type: node.type || 'Unknown',
            loader: node.loader,
            properties: node.properties || {},
            color: nodeColorProvider(node.type),
          },
        });
      }
    });

    edges.forEach((edge) => {
      if (!edge || !edge.source || !edge.target) return;
      const edgeId = `${edge.source}-${edge.target}-${edge.label || ''}`;
      if (this.cy!.getElementById(edgeId).length === 0 && !addedIds.has(edgeId)) {
        addedIds.add(edgeId);
        newElements.push({
          group: 'edges',
          data: {
            id: edgeId,
            source: edge.source,
            target: edge.target,
            label: edge.label,
          },
        });
      }
    });

    if (newElements.length > 0) {
      try {
        this.cy.add(newElements);
      } catch (e) {
        console.error('[Cytoscape] Failed to add elements:', e);
      }

      this.cy.resize();

      if (this.cy.nodes().length > 0 && this.container.offsetWidth > 0) {
        const hasEdges = this.cy.edges().length > 0;
        const layoutOptions = hasEdges
          ? {
              name: 'cose',
              animate: false,
              fit: true,
              padding: 30,
              randomize: false,
              nodeRepulsion: () => 400000,
              idealEdgeLength: () => 100,
              edgeElasticity: () => 100,
            }
          : {
              name: 'grid',
              animate: false,
              fit: true,
              padding: 30,
            };

        try {
          const layout = this.cy.layout(layoutOptions);
          layout.run();
        } catch (e) {
          console.error('[Cytoscape] Layout failed:', e);
        }
      }

      this.cy.fit(undefined, 30);
    }
  }

  onNodeClick(handler: (node: string, attributes: ContextGraphNode) => void) {
    this.cy?.on('tap', 'node', (evt) => {
      const data = evt.target.data();
      const attrs: ContextGraphNode = {
        id: data.id,
        label: data.label,
        type: data.type,
        loader: data.loader,
        properties: data.properties,
      };
      handler(data.id, attrs);
    });
  }

  onNodeHover() {
    this.cy?.on('mouseover', 'node', (evt) => {
      const data = evt.target.data();
      this.container.title = data.label || data.id;
    });

    this.cy?.on('mouseout', 'node', () => {
      this.container.title = '';
    });
  }

  private truncateLabel(label: string, maxLength = 20): string {
    if (label.length <= maxLength) return label;
    const half = Math.floor((maxLength - 1) / 2);
    return `${label.slice(0, half)}…${label.slice(-half)}`;
  }

  clear() {
    this.cy?.elements().remove();
  }

  kill() {
    this.cy?.destroy();
    this.cy = undefined;
  }

  refresh() {
    this.cy?.resize();
    this.cy?.fit();
  }

  zoomIn() {
    if (this.cy) {
      this.cy.zoom(this.cy.zoom() * 1.2);
    }
  }

  zoomOut() {
    if (this.cy) {
      this.cy.zoom(this.cy.zoom() * 0.8);
    }
  }

  center() {
    if (this.cy) {
      this.cy.fit(undefined, 30);
    }
  }
}
