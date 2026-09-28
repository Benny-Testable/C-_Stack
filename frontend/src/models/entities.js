export function emptyStudent() {
  return {
    firstName: '',
    lastName: '',
    email: '',
    passwordHash: '',
    phone: '',
    dateOfBirth: '2004-01-15',
    address: '',
    city: '',
    residency: 'InState',
    gpa: 3.2,
    major: 'Biology',
    enrollmentYear: 2024,
    creditHours: 30,
    annualIncome: 24000,
    isActive: true,
    notes: ''
  }
}

export function emptyScholarship() {
  return {
    name: '',
    sponsor: '',
    description: '',
    categoryId: 1,
    awardAmount: 2500,
    minimumGpa: 3,
    minimumCreditHours: 12,
    maximumIncome: 60000,
    requiredMajor: 'Any',
    requiredResidency: 'Any',
    requiresEssay: true,
    requiresTranscript: true,
    openDate: '2026-01-01',
    deadline: '2026-12-31',
    seats: 10,
    isActive: true
  }
}

export function emptyApplication() {
  return {
    studentId: 0,
    scholarshipId: 0,
    essayText: '',
    requestedAmount: 0
  }
}

export function emptyDocument() {
  return {
    applicationId: 0,
    fileName: '',
    documentType: 'Transcript',
    status: 'Pending',
    isRequired: true,
    notes: ''
  }
}
