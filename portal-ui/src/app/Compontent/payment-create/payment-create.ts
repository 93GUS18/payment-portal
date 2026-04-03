import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatDialogModule, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { HttpClient } from '@angular/common/http';
import { Payment } from '../../Interface/payment';
import { ApiResponse } from '../../Interface/api-response';
import { environment } from '../../../environments/environtment';

const PAYMENT_API_HOST = environment.PAYMENT_API_HOST;

export interface DialogData {
  payment?: Payment;
  mode: 'view' | 'edit' | 'create';
  reference?: string;
}

@Component({
  selector: 'app-payment-create',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './payment-create.html',
  styleUrl: './payment-create.css',
})
export class PaymentCreate {
  form: FormGroup;
  isLoading = false;
  mode: 'view' | 'edit' | 'create';
  currencies = [
    { value: 1, label: 'EUR' },
    { value: 2, label: 'GBP' },
    { value: 3, label: 'INR' },
    { value: 4, label: 'USD' }
  ];

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    public dialogRef: MatDialogRef<PaymentCreate>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData
  ) {
    this.mode = data.mode;
    this.form = this.fb.group({
      Reference: ['', [Validators.required, Validators.minLength(3)]],
      Amount: ['', [Validators.required, Validators.pattern(/^\d+(\.\d{1,2})?$/), Validators.max(2147483647)]],
      Currency: [1, Validators.required]
    });

    if (data.payment && (data.mode === 'view' || data.mode === 'edit')) {
      this.form.patchValue({
        Reference: data.payment.reference,
        Amount: data.payment.amount,
        Currency: data.payment.currency
      });
    }

    if (data.mode === 'create' && data.reference) {
      this.form.patchValue({
        Reference: data.reference
      });
      this.form.get('Reference')?.disable();
    }

    if (data.mode === 'edit') {
      this.form.get('Reference')?.disable();
    }

    if (data.mode === 'view') {
      this.form.disable();
    }
  }

  onSubmit() {
    if (this.form.invalid) {
      return;
    }

    this.isLoading = true;
    const formData = this.form.getRawValue();

    if (this.mode === 'create') {
      this.http.post<ApiResponse<Payment>>(PAYMENT_API_HOST + 'api/payments', formData)
        .subscribe({
          next: (response) => {
            if (response.statusCode === 200 || response.statusCode === 201) {
              this.dialogRef.close({ success: true, data: response.data, mode: 'create' });
            } else {
              console.error('Error creating payment:', response.errorMessage);
            }
            this.isLoading = false;
          },
          error: (error) => {
            console.error('Error creating payment:', error);
            this.isLoading = false;
          }
        });
    } else if (this.mode === 'edit') {
      const paymentId = this.data.payment?.id;
      this.http.put<ApiResponse<Payment>>(PAYMENT_API_HOST + 'api/payments/' + paymentId, formData)
        .subscribe({
          next: (response) => {
            if (response.statusCode === 200) {
              this.dialogRef.close({ success: true, data: response.data, mode: 'edit' });
            } else {
              console.error('Error updating payment:', response.errorMessage);
            }
            this.isLoading = false;
          },
          error: (error) => {
            console.error('Error updating payment:', error);
            this.isLoading = false;
          }
        });
    }
  }

  onCancel() {
    this.dialogRef.close();
  }

  getTitle() {
    if (this.mode === 'create') return 'Create Payment';
    if (this.mode === 'edit') return 'Edit Payment';
    return 'View Payment';
  }

  getSubmitButtonText() {
    if (this.mode === 'view') return null;
    return this.mode === 'create' ? 'Create' : 'Update';
  }
}
