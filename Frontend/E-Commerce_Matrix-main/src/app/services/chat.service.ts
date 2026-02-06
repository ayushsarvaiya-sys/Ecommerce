import { Injectable, signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject, Observable } from 'rxjs';

export interface ChatMessage {
  id: number;
  userId: string;
  userName: string;
  message: string;
  timestamp: Date;
  userProfileImage?: string;
}

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private hubConnection: signalR.HubConnection | undefined;
  private messagesSubject = new BehaviorSubject<ChatMessage[]>([]);
  public messages$ = this.messagesSubject.asObservable();

  private connectionStatusSubject = new BehaviorSubject<boolean>(false);
  public connectionStatus$ = this.connectionStatusSubject.asObservable();

  private usersCountSubject = new BehaviorSubject<number>(0);
  public usersCount$ = this.usersCountSubject.asObservable();

  constructor() {}

  public startConnection(): Promise<void> {
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) {
      console.log('Already connected');
      return Promise.resolve();
    }

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7067/api/chat', {
        withCredentials: true,
        skipNegotiation: false,
        transport: signalR.HttpTransportType.WebSockets
      })
      .withAutomaticReconnect([0, 2000, 5000, 10000])
      .build();

    // Setup listeners BEFORE starting connection
    this.setupListeners();

    return this.hubConnection.start()
      .then(() => {
        this.connectionStatusSubject.next(true);
        console.log('SignalR Connected - Status emitted');
        this.getChatHistory();
      })
      .catch((err) => {
        console.error('Error connecting to SignalR:', err);
        this.connectionStatusSubject.next(false);
        return Promise.reject(err);
      });
  }

  private setupListeners(): void {
    if (!this.hubConnection) return;

    this.hubConnection.on('ReceiveMessage', (message: ChatMessage) => {
      const currentMessages = this.messagesSubject.value;
      message.timestamp = new Date(message.timestamp);
      this.messagesSubject.next([...currentMessages, message]);
    });

    this.hubConnection.on('ChatHistory', (messages: ChatMessage[]) => {
      const formattedMessages = messages.map(msg => ({
        ...msg,
        timestamp: new Date(msg.timestamp)
      }));
      this.messagesSubject.next(formattedMessages);
    });

    this.hubConnection.on('UserConnected', (data: any) => {
      console.log('User connected:', data.message);
    });

    this.hubConnection.on('UserDisconnected', (data: any) => {
      console.log('User disconnected:', data.message);
    });

    this.hubConnection.onreconnected(() => {
      console.log('Reconnected to SignalR');
      this.connectionStatusSubject.next(true);
      this.getChatHistory();
    });

    this.hubConnection.onreconnecting(() => {
      console.log('Attempting to reconnect...');
      this.connectionStatusSubject.next(false);
    });

    this.hubConnection.onclose(() => {
      console.log('Connection closed');
      this.connectionStatusSubject.next(false);
    });
  }

  public sendMessage(userId: string, userName: string, message: string, profileImage?: string): Promise<void> | undefined {
    if (!this.hubConnection || !message.trim()) {
      return;
    }

    return this.hubConnection.invoke('SendMessage', userId, userName, message, profileImage)
      .catch((err) => console.error('Error sending message:', err));
  }

  private getChatHistory(): void {
    if (!this.hubConnection) return;

    this.hubConnection.invoke('GetChatHistory')
      .catch((err) => console.error('Error getting chat history:', err));
  }

  public stopConnection(): Promise<void> | undefined {
    if (!this.hubConnection) {
      return;
    }

    return this.hubConnection.stop()
      .then(() => {
        this.connectionStatusSubject.next(false);
        console.log('SignalR Disconnected');
      })
      .catch((err) => console.error('Error disconnecting from SignalR:', err));
  }

  public isConnected(): boolean {
    return this.hubConnection?.state === signalR.HubConnectionState.Connected;
  }
}
