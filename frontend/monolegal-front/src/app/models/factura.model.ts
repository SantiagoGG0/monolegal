export interface Factura {
  id: string;
  clienteId: string;
  clienteNombre: string;
  clienteEmail: string;
  numero: string;
  monto: number;
  fechaEmision: string;
  fechaVencimiento: string;
  estado: string;
  descripcion: string;
}
