import { Component, ElementRef, OnInit, ViewChildren, QueryList, AfterViewInit } from '@angular/core';
import Hls from 'hls.js';

interface CameraStream {
  id: string;
  nome: string;
  url: string;
  status: 'Conectando' | 'Ao Vivo' | 'Offline';
}

@Component({
  selector: 'app-mosaico',
  templateUrl: './mosaico.component.html',
  styleUrls: ['./mosaico.component.scss'],
  standalone: false
})
export class MosaicoComponent implements OnInit, AfterViewInit {
  @ViewChildren('videoElement') videoElements!: QueryList<ElementRef<HTMLVideoElement>>;

  // Mock de Câmeras para demonstração do Mosaico (a URL real virá da API na Fase 5.2)
  cameras: CameraStream[] = [
    { id: '1', nome: 'Câmera Frontal - Entrada', url: 'https://test-streams.mux.dev/x36xhzz/x36xhzz.m3u8', status: 'Conectando' },
    { id: '2', nome: 'Pátio Traseiro', url: 'https://test-streams.mux.dev/x36xhzz/x36xhzz.m3u8', status: 'Conectando' },
    { id: '3', nome: 'Recepção (LPR)', url: 'https://test-streams.mux.dev/x36xhzz/x36xhzz.m3u8', status: 'Conectando' },
    { id: '4', nome: 'Corredor Sul', url: 'https://test-streams.mux.dev/x36xhzz/x36xhzz.m3u8', status: 'Conectando' }
  ];

  constructor() { }

  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
    this.iniciarStreams();
  }

  iniciarStreams(): void {
    this.videoElements.forEach((videoRef, index) => {
      const video = videoRef.nativeElement;
      const camera = this.cameras[index];

      if (Hls.isSupported()) {
        const hls = new Hls({
          maxBufferLength: 30,
          maxMaxBufferLength: 600,
        });
        hls.loadSource(camera.url);
        hls.attachMedia(video);
        hls.on(Hls.Events.MANIFEST_PARSED, () => {
          video.play().catch(e => console.error('Erro no autoplay:', e));
          setTimeout(() => camera.status = 'Ao Vivo', 500); // UI Feedback
        });
        hls.on(Hls.Events.ERROR, () => {
          camera.status = 'Offline';
        });
      }
      else if (video.canPlayType('application/vnd.apple.mpegurl')) {
        // Fallback for Safari
        video.src = camera.url;
        video.addEventListener('loadedmetadata', () => {
          video.play();
          camera.status = 'Ao Vivo';
        });
      }
    });
  }
}
