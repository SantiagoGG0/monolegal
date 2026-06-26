import { Component, OnInit } from '@angular/core';
import { CommonModule, DecimalPipe, DatePipe } from '@angular/common';
import { FacturaService } from '../../services/factura.service';
import { ClienteResumen } from '../../models/cliente-resumen.model';
import { NotificacionResult } from '../../models/notificacion-result.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, DecimalPipe, DatePipe],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  clientes: ClienteResumen[] = [];
  resultados: NotificacionResult[] = [];
  cargando = false;
  procesando = false;
  mostrarResultados = false;
  error: string | null = null;

  constructor(private facturaService: FacturaService) {}

  ngOnInit(): void {
    this.cargarResumen();
  }

  cargarResumen(): void {
    this.cargando = true;
    this.error = null;
    this.facturaService.getResumen().subscribe({
      next: (data) => {
        this.clientes = data;
        this.cargando = false;
      },
      error: () => {
        this.error = 'No se pudo conectar con el servidor. Verifique que la API esté corriendo en http://localhost:5000.';
        this.cargando = false;
      }
    });
  }

  procesarNotificaciones(): void {
    this.procesando = true;
    this.mostrarResultados = false;
    this.facturaService.procesarNotificaciones().subscribe({
      next: (data) => {
        this.resultados = data;
        this.procesando = false;
        this.mostrarResultados = true;
        this.cargarResumen();
      },
      error: () => {
        this.error = 'Error al procesar notificaciones.';
        this.procesando = false;
      }
    });
  }

  estadoBadgeClass(estado: string): string {
    const map: Record<string, string> = {
      activo: 'badge-activo',
      primerrecordatorio: 'badge-primer',
      segundorecordatorio: 'badge-segundo',
      desactivado: 'badge-desactivado'
    };
    return map[estado] ?? 'badge-default';
  }

  estadoLabel(estado: string): string {
    const map: Record<string, string> = {
      activo: 'Activo',
      primerrecordatorio: '1er Recordatorio',
      segundorecordatorio: '2do Recordatorio',
      desactivado: 'Desactivado'
    };
    return map[estado] ?? estado;
  }
}
