import { Component, OnInit } from '@angular/core';
import { ApiClientService, Camera } from '../../services/api-client.service';

@Component({
  selector: 'app-cameras',
  templateUrl: './cameras.component.html',
  styleUrl: './cameras.component.scss',
  standalone: false
})
export class CamerasComponent implements OnInit {
  cameras: Camera[] = [];
  
  // Modelos do Form
  novaCameraNome = '';
  novaCameraUrl = '';

  constructor(private api: ApiClientService) { }

  ngOnInit() {
    this.carregarCameras();
  }

  carregarCameras() {
    this.api.getCameras().subscribe({
      next: (dados) => this.cameras = dados,
      error: (err) => console.error('Erro ao buscar câmeras:', err)
    });
  }

  adicionar() {
    if (!this.novaCameraNome || !this.novaCameraUrl) return;

    const req: Camera = {
      nome: this.novaCameraNome,
      urlRtsp: this.novaCameraUrl,
      ativa: true
    };

    this.api.criarCamera(req).subscribe({
      next: () => {
        this.novaCameraNome = '';
        this.novaCameraUrl = '';
        this.carregarCameras(); // Atualiza a grid
      },
      error: (err) => console.error('Erro ao criar câmera:', err)
    });
  }

  remover(id: string) {
    if (confirm('Tem certeza que deseja excluir esta câmera?')) {
      this.api.excluirCamera(id).subscribe(() => this.carregarCameras());
    }
  }
}
