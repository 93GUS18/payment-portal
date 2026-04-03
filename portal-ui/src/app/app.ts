import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { PaymentList } from './Compontent/payment-list/payment-list';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, PaymentList],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('portal-ui');
}
