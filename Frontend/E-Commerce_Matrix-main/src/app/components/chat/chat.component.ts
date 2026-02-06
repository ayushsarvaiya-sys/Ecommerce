import { Component, OnInit, OnDestroy, ViewChild, ElementRef, AfterViewChecked, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChatService, ChatMessage } from '../../services/chat.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit, OnDestroy, AfterViewChecked {
  @ViewChild('messageContainer') messageContainer!: ElementRef;

  messages: ChatMessage[] = [];
  newMessage: string = '';
  isConnected: boolean = false;
  isLoading: boolean = true;
  currentUser = {
    id: '',
    name: 'Anonymous User',
    profileImage: ''
  };

  private subscriptions: Subscription[] = [];

  constructor(private chatService: ChatService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    console.log('ChatComponent: Initializing...');
    this.loadCurrentUser();
    this.subscribeToMessages();
    this.subscribeToConnectionStatus();
    this.initializeConnection();
  }

  ngOnDestroy(): void {
    console.log('ChatComponent: Destroying...');
    this.subscriptions.forEach(sub => sub.unsubscribe());
    this.chatService.stopConnection();
  }

  ngAfterViewChecked(): void {
    this.scrollToBottom();
  }

  private loadCurrentUser(): void {
    const userStr = localStorage.getItem('user');
    if (userStr) {
      try {
        const user = JSON.parse(userStr);
        this.currentUser = {
          id: user.id || user.userId || 'anonymous',
          name: user.firstName && user.lastName 
            ? `${user.firstName} ${user.lastName}` 
            : user.email || 'Anonymous User',
          profileImage: user.profileImage || ''
        };
        console.log('ChatComponent: User loaded:', this.currentUser.name);
      } catch (e) {
        console.error('ChatComponent: Error parsing user:', e);
      }
    }
  }

  private subscribeToMessages(): void {
    const messagesSub = this.chatService.messages$.subscribe((messages) => {
      this.messages = messages;
      console.log('ChatComponent: Messages updated, count:', messages.length);
      this.cdr.detectChanges();
    });
    this.subscriptions.push(messagesSub);
  }

  private subscribeToConnectionStatus(): void {
    const statusSub = this.chatService.connectionStatus$.subscribe((status) => {
      console.log('ChatComponent: Connection status changed to:', status);
      this.isConnected = status;
      this.cdr.detectChanges();
    });
    this.subscriptions.push(statusSub);
  }

  private initializeConnection(): void {
    this.isLoading = true;
    this.cdr.detectChanges();
    
    console.log('ChatComponent: Starting connection...');
    this.chatService.startConnection()
      .then(() => {
        console.log('ChatComponent: Connection successful');
        this.isConnected = true;
        this.cdr.detectChanges();
      })
      .catch((error) => {
        console.error('ChatComponent: Connection error:', error);
        this.isConnected = false;
        this.cdr.detectChanges();
      })
      .finally(() => {
        this.isLoading = false;
        console.log('ChatComponent: Loading finished', { isConnected: this.isConnected, isLoading: this.isLoading });
        this.cdr.detectChanges();
      });
  }

  private scrollToBottom(): void {
    try {
      if (this.messageContainer) {
        this.messageContainer.nativeElement.scrollTop = 
          this.messageContainer.nativeElement.scrollHeight;
      }
    } catch (err) {
      console.error('Error scrolling to bottom:', err);
    }
  }

  public sendMessage(): void {
    if (!this.newMessage.trim() || !this.isConnected) {
      return;
    }

    console.log('ChatComponent: Sending message:', this.newMessage);
    this.chatService.sendMessage(
      this.currentUser.id,
      this.currentUser.name,
      this.newMessage,
      this.currentUser.profileImage
    );

    this.newMessage = '';
  }

  public onKeyPress(event: KeyboardEvent): void {
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      this.sendMessage();
    }
  }

  public formatTime(date: Date): string {
    const messageDate = new Date(date);
    const now = new Date();
    const diffMs = now.getTime() - messageDate.getTime();
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMins / 60);
    const diffDays = Math.floor(diffHours / 24);

    if (diffMins < 1) return 'Just now';
    if (diffMins < 60) return `${diffMins}m ago`;
    if (diffHours < 24) return `${diffHours}h ago`;
    if (diffDays < 7) return `${diffDays}d ago`;

    return messageDate.toLocaleDateString();
  }

  public isCurrentUser(userId: string): boolean {
    return userId === this.currentUser.id;
  }
}
