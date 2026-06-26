import { Factura } from './factura.model';

export interface ClienteResumen {
  clienteId: string;
  clienteNombre: string;
  clienteEmail: string;
  totalFacturas: number;
  montoTotal: number;
  facturas: Factura[];
}
