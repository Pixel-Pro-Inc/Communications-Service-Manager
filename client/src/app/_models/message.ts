
export interface Message {
  id: number;
  subject:string;
  recipients: string[];
  content: string;
  WhatsappOnly:boolean;
}
