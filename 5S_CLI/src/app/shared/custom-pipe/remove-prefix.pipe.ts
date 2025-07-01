import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'removePrefix'
})
export class RemovePrefixPipe implements PipeTransform {
  transform(value: string | undefined | null): string {
    if (!value) return '-';
    return value.replace(/^L\d+\s+/, '').trim();
  }
}
