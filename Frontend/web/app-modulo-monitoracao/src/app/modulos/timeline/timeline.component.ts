import { Component, OnInit } from '@angular/core';
import { ApiClientService, Gravacao } from '../../services/api-client.service';

@Component({
  selector: 'app-timeline',
  templateUrl: './timeline.component.html',
  styleUrl: './timeline.component.scss',
  standalone: false
})
export class TimelineComponent implements OnInit {
  gravacoes: Gravacao[] = [];

  constructor(private api: ApiClientService) {}

  ngOnInit(): void {
    this.api.getGravacoes().subscribe({
      next: (dados) => this.gravacoes = dados,
      error: (err) => console.error('Erro ao buscar gravações', err)
    });
  }
}
