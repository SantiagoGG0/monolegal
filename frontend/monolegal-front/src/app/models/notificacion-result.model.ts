export interface NotificacionResult {
  facturaId: string;
  clienteEmail: string;
  estadoAnterior: string;
  estadoNuevo: string;
  emailEnviado: boolean;
  error?: string;
}
