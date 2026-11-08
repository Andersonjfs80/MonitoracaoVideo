import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Camera {
  id?: string;
  nome: string;
  urlRtsp: string;
  ativa: boolean;
}

export interface Gravacao {
  id: string;
  cameraId: string;
  inicio: string;
  fim: string;
  caminhoArquivo: string;
}

@Injectable({
  providedIn: 'root'
})
export class ApiClientService {
  // A URL deve apontar para o app-backend-monitoracao-video
  private baseUrl = 'http://localhost:5000/api'; 

  constructor(private http: HttpClient) { }

  private getHeaders(): HttpHeaders {
    // Cabeçalho obrigatório de segurança exigido pela arquitetura
    return new HttpHeaders({
      'X-Sec-Key': '1',
      'Content-Type': 'application/json'
    });
  }

  // --- Câmeras ---
  getCameras(): Observable<Camera[]> {
    return this.http.get<Camera[]>(`${this.baseUrl}/cameras`, { headers: this.getHeaders() });
  }

  criarCamera(camera: Camera): Observable<Camera> {
    return this.http.post<Camera>(`${this.baseUrl}/cameras`, camera, { headers: this.getHeaders() });
  }

  excluirCamera(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/cameras/${id}`, { headers: this.getHeaders() });
  }

  // --- Linha do Tempo (Gravações) ---
  getGravacoes(): Observable<Gravacao[]> {
    return this.http.get<Gravacao[]>(`${this.baseUrl}/gravacoes`, { headers: this.getHeaders() });
  }
}
