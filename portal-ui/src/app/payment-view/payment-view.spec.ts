import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PaymentView } from './payment-view';

describe('PaymentView', () => {
  let component: PaymentView;
  let fixture: ComponentFixture<PaymentView>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PaymentView],
    }).compileComponents();

    fixture = TestBed.createComponent(PaymentView);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
