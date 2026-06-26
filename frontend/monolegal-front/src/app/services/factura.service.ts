import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { ClienteResumen } from '../models/cliente-resumen.model';
import { NotificacionResult } from '../models/notificacion-result.model';

@Injectable({
  providedIn: 'root'
})
export class FacturaService {
  private readonly base = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getResumen(): Observable<ClienteResumen[]> {
    return this.http.get<ClienteResumen[]>(`${this.base}/facturas/resumen`);
  }

  procesarNotificaciones(): Observable<NotificacionResult[]> {
    return this.http.post<NotificacionResult[]>(`${this.base}/notificaciones/procesar`, {});
  }
}
