import { Component, ViewChild, AfterViewInit, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatPaginatorModule, MatPaginator } from '@angular/material/paginator';
import { MatSortModule, MatSort } from '@angular/material/sort';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { HttpClient } from '@angular/common/http';
import { catchError, finalize } from 'rxjs/operators';
import { of } from 'rxjs';
import { Payment } from '../../Interface/payment';
import { environment } from '../../../environments/environtment';

const PAYMENT_API_HOST = environment.PAYMENT_API_HOST;

@Component({
  selector: 'app-payment-list',
  imports: [CommonModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatIconModule,
    MatButtonModule,
    MatSnackBarModule],
  templateUrl: './payment-list.html',
  styleUrl: './payment-list.css',
})
export class PaymentList {
  constructor(private snackBar: MatSnackBar, private http: HttpClient) {}

  displayedColumns: string[] = ['Reference', 'Currency', 'Amount', 'CreatedAt', 'actions'];
  dataSource = new MatTableDataSource<Payment>();
  isLoading = false;
  selectedRow: Payment | null = null;

  loadPayments() {
    this.isLoading = true;
    this.http.get<Payment[]>(PAYMENT_API_HOST + 'api/payments')
      .pipe(
        catchError(error => {
          console.error('Error loading payments:', error);
          this.showError('Failed to load payments. Please try again.');
          return of([]);
        }),
        finalize(() => this.isLoading = false)
      )
      .subscribe(data => {
        this.dataSource.data = data;
        this.showSuccess('Payments loaded successfully!');
      });
  }
  
  ngAfterViewInit() {
    setTimeout(() => {
      this.isLoading = false;
    });
  }

  ngOnInit() {
    this.loadPayments();
  }

  refreshData() {
    this.loadPayments();
  }

  addPayment(payment: Payment){

  }

  updaePayment(payment: Payment) {
  }

  deletePayment(payment: Payment) {
  }

  private showSuccess(message: string) {
    this.snackBar.open(message, 'Close', { duration: 3000, panelClass: ['success-snackbar'] });
  }

  private showError(message: string) {
    this.snackBar.open(message, 'Close', { duration: 5000, panelClass: ['error-snackBar'] });
  }

  private showInfo(message: string) {
    this.snackBar.open(message, 'Close', { duration: 2000 });
  }
}
