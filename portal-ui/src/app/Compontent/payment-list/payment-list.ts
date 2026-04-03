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
import { MatMenuModule } from '@angular/material/menu';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { HttpClient } from '@angular/common/http';
import { catchError, finalize } from 'rxjs/operators';
import { of } from 'rxjs';
import { Payment } from '../../Interface/payment';
import { ApiResponse } from '../../Interface/api-response';
import { PaymentCreate, DialogData } from '../payment-create/payment-create';
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
    MatSnackBarModule,
    MatDialogModule,
    MatMenuModule],
  templateUrl: './payment-list.html',
  styleUrl: './payment-list.css',
})
export class PaymentList implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private snackBar: MatSnackBar,
    private http: HttpClient,
    private dialog: MatDialog
  ) {}

  displayedColumns: string[] = ['Reference', 'Currency', 'Amount', 'CreatedAt', 'actions'];
  dataSource = new MatTableDataSource<Payment>();
  isLoading = false;
  selectedRow: Payment | null = null;

  currencies = [
    { value: 1, label: 'EUR' },
    { value: 2, label: 'GBP' },
    { value: 3, label: 'INR' },
    { value: 4, label: 'USD' }
  ];

  loadPayments() {
    this.isLoading = true;
    this.http.get<ApiResponse<Payment[]>>(PAYMENT_API_HOST + 'api/payments')
      .pipe(
        catchError(error => {
          console.log('Error loading payments:', error);
          this.showError('Failed to load payments. Please try again.');
          return of({ statusCode: 500, message: '', data: [], errorMessage: '' });
        }),
        finalize(() => this.isLoading = false)
      )
      .subscribe(response => {
        if (response.statusCode === 200 && response.data) {
          this.dataSource.data = response.data;
          this.showSuccess('Payments loaded successfully!');
        } else {
          this.dataSource.data = [];
          this.showError(response.errorMessage || 'Failed to load payments.');
        }
      });
  }
  
  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  ngOnInit() {
    this.loadPayments();
  }

  refreshData() {
    this.selectedRow = null;
    this.loadPayments();
  }

  getCurrencyLabel(currencyValue: number): string {
    const currency = this.currencies.find(c => c.value === currencyValue);
    return currency ? currency.label : 'Unknown';
  }

  viewPayment(payment: Payment) {
    const dialogData: DialogData = {
      payment,
      mode: 'view'
    };

    this.dialog.open(PaymentCreate, {
      width: '400px',
      data: dialogData
    });
  }

  editPayment(payment: Payment) {
    const dialogData: DialogData = {
      payment,
      mode: 'edit'
    };

    const dialogRef = this.dialog.open(PaymentCreate, {
      width: '400px',
      data: dialogData
    });

    dialogRef.afterClosed().subscribe((result: any) => {
      if (result && result.success && result.mode === 'edit') {
        this.showSuccess('Payment updated successfully!');
        this.loadPayments();
      }
    });
  }

  addPayment() {
    this.isLoading = true;
    this.http.get<ApiResponse<number>>(PAYMENT_API_HOST + 'api/payments/next-id')
      .pipe(
        finalize(() => this.isLoading = false)
      )
      .subscribe({
        next: (response) => {
          if (response.statusCode === 200 && response.data) {
            const nextId = response.data;
            const today = new Date();
            const dateStr = today.getFullYear() + 
                            ('0' + (today.getMonth() + 1)).slice(-2) + 
                            ('0' + today.getDate()).slice(-2);
            const ref = `PAY-${dateStr}-${nextId.toString().padStart(5, '0')}`;

            const dialogData: DialogData = {
              mode: 'create',
              reference: ref
            };

            const dialogRef = this.dialog.open(PaymentCreate, {
              width: '400px',
              data: dialogData
            });

            dialogRef.afterClosed().subscribe((result: any) => {
              if (result && result.success && result.mode === 'create') {
                this.showSuccess('Payment created successfully!');
                this.loadPayments();
              }
            });
          }
        },
        error: (error) => {
          console.error('Error retrieving next ID:', error);
          this.showError('Failed to create payment. Please try again.');
        }
      });
  }

  deletePayment(payment: Payment) {
    const snackBarRef = this.snackBar.open(`Delete payment ${payment.reference}?`, 'Delete', { duration: 10000 });

    snackBarRef.onAction().subscribe(() => {
      this.isLoading = true;
      this.http.delete(PAYMENT_API_HOST + 'api/payments/' + payment.id)
        .pipe(
          finalize(() => this.isLoading = false)
        )
        .subscribe({
          next: () => {
            this.showSuccess('Payment deleted successfully!');
            this.loadPayments();
          },
          error: (error) => {
            console.error('Error deleting payment:', error);
            this.showError('Failed to delete payment. Please try again.');
          }
        });
    });
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
